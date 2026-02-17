using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace WeCare.Handlers;

public class TenantCredentialSyncHandler : 
    ILocalEventHandler<EntityUpdatedEventData<Tenant>>,
    ITransientDependency
{
    private readonly IdentityUserManager _userManager;
    private readonly ICurrentTenant _currentTenant;
    private readonly ILogger<TenantCredentialSyncHandler> _logger;

    public TenantCredentialSyncHandler(
        IdentityUserManager userManager,
        ICurrentTenant currentTenant,
        ILogger<TenantCredentialSyncHandler> logger)
    {
        _userManager = userManager;
        _currentTenant = currentTenant;
        _logger = logger;
    }

    public async Task HandleEventAsync(EntityUpdatedEventData<Tenant> eventData)
    {
        var tenant = eventData.Entity;
        
        // Obter as propriedades extras que definimos
        var adminEmail = tenant.GetProperty<string>("AdminEmail");
        var adminPassword = tenant.GetProperty<string>("AdminPassword");

        if (string.IsNullOrWhiteSpace(adminEmail) && string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        try
        {
            _logger.LogInformation($"Iniciando sincronização de credenciais para o tenant {tenant.Name} ({tenant.Id})");

            // Trocar para o contexto do tenant para manipular o usuário administrativo local
            using (_currentTenant.Change(tenant.Id))
            {
                // Tenta encontrar o usuário administrador por nome, email ou pela role Admin
                Volo.Abp.Identity.IdentityUser adminUser = null;

                // 1. Prioridade: Encontrar pelo Email alvo (se já existir um usuário com esse email, usamos ele)
                if (!string.IsNullOrWhiteSpace(adminEmail)) 
                {
                    adminUser = await _userManager.FindByEmailAsync(adminEmail);
                }

                // 2. Se não encontrou, tenta ncontrar pelo usuário padrão "admin" (para renomear/atualizar)
                if (adminUser == null)
                {
                    adminUser = await _userManager.FindByNameAsync("admin");
                }
                
                // 3. Se ainda não encontrou, busca qualquer usuário da role 'admin'
                if (adminUser == null)
                {
                    var adminRoleUsers = await _userManager.GetUsersInRoleAsync("admin");
                    adminUser = adminRoleUsers.FirstOrDefault();
                }
                
                if (adminUser != null)
                {
                    _logger.LogInformation($"Usuário admin encontrado: {adminUser.UserName} ({adminUser.Id}). Sincronizando dados...");
                    bool changed = false;

                    // Sincronizar Email se houver alteração
                    if (!string.IsNullOrWhiteSpace(adminEmail) && adminUser.Email != adminEmail)
                    {
                        (await _userManager.SetEmailAsync(adminUser, adminEmail)).CheckErrors();
                        (await _userManager.SetUserNameAsync(adminUser, adminEmail)).CheckErrors(); 
                        changed = true;
                        _logger.LogInformation($"Email/Username atualizado para: {adminEmail}");
                    }

                    // Sincronizar Senha se houver alteração
                    if (!string.IsNullOrWhiteSpace(adminPassword))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(adminUser);
                        var result = await _userManager.ResetPasswordAsync(adminUser, token, adminPassword);
                        
                        if (!result.Succeeded)
                        {
                            var errors = string.Join("\n", result.Errors.Select(e => e.Description));
                            _logger.LogError($"Erro ao atualizar senha do admin: {errors}");
                            
                            // Usando BusinessException com mensagem e detalhes separados
                            // O ABP UI mostra a Mensagem como título/resumo e os Detalhes no corpo do modal
                            throw new BusinessException(
                                code: "WeCare:AdminPasswordComplexity",
                                message: "A senha do administrador não atende aos requisitos de segurança.",
                                details: errors
                            );
                        }
                        
                        changed = true;
                        _logger.LogInformation("Senha do administrador atualizada com sucesso.");
                    }

                    if (changed)
                    {
                        (await _userManager.UpdateAsync(adminUser)).CheckErrors();
                    }
                }
                else
                {
                    _logger.LogWarning($"Nenhum usuário administrador encontrado para o tenant {tenant.Name}");
                }
            }
        }
        catch (BusinessException)
        {
            throw; // Re-throw para o UI (captura BusinessException e UserFriendlyException)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro inesperado ao sincronizar credenciais para o tenant {tenant.Name} ({tenant.Id})");
            throw new BusinessException(
                code: "WeCare:SyncError",
                message: "Ocorreu um erro ao sincronizar as credenciais do administrador.",
                details: "Consulte os logs do servidor para mais detalhes ou verifique se os dados digitados são válidos."
            );
        }
    }
}
