using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using WeCare.Permissions;
using WeCare.Shared;

// Apelido para o usuário de identidade para clareza
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace WeCare.Therapists
{
    public class TherapistAppService : CrudAppService<
            Therapist,
            TherapistDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateTherapistDto>,
        ITherapistAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IIdentityRoleRepository _roleRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public TherapistAppService(
            IRepository<Therapist, Guid> repository,
            IdentityUserManager userManager,
            IIdentityRoleRepository roleRepository,
            IUnitOfWorkManager unitOfWorkManager) // Adicionamos o IUnitOfWorkManager
            : base(repository)
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
            _unitOfWorkManager = unitOfWorkManager;

            // Permissões padrão do CrudAppService
            GetPolicyName = WeCarePermissions.Therapists.Default;
            GetListPolicyName = WeCarePermissions.Therapists.Default;
            CreatePolicyName = WeCarePermissions.Therapists.Create;
            UpdatePolicyName = WeCarePermissions.Therapists.Edit;
            DeletePolicyName = WeCarePermissions.Therapists.Delete;
        }

        // Sobrescrevemos o CreateAsync para orquestrar a criação do usuário.
        [Authorize(WeCarePermissions.Therapists.Create)]
        public override async Task<TherapistDto> CreateAsync(CreateUpdateTherapistDto input)
        {
            var existingUser = await _userManager.FindByNameAsync(input.UserName);
            if (existingUser != null)
            {
                throw new UserFriendlyException("Já existe um usuário com este nome.");
            }

            // 1. Criar o usuário de identidade.
            var user = new IdentityUser(GuidGenerator.Create(), input.UserName, input.Email)
            {
                Name = input.Name
            };
            (await _userManager.CreateAsync(user, input.Password)).CheckErrors();

            // 2. Atribuir a role "Terapeuta".
            const string therapistRole = "Terapeuta";
            (await _userManager.AddToRoleAsync(user, therapistRole)).CheckErrors();

            // 3. Salvar as mudanças para disparar o evento.
            // O EventHandler será executado dentro desta unidade de trabalho.
            if (_unitOfWorkManager.Current != null)
            {
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }

            // 4. Buscar a entidade Therapist que foi criada pelo EventHandler.
            var newTherapist = await Repository.FirstOrDefaultAsync(t => t.UserId == user.Id);
            if (newTherapist == null)
            {
                // Fallback: This should ideally catch the race condition if EventHandler hasn't fired yet
                // But since we called SaveChangesAsync, it should be there.
                throw new UserFriendlyException("Falha ao criar terapeuta. O usuário foi criado, mas o registro de terapeuta não.");
            }

            // 5. Mapear e retornar o DTO.
            return ObjectMapper.Map<Therapist, TherapistDto>(newTherapist);
        }

        // Função de lookup para dropdowns (permanece a mesma).
        public async Task<ListResultDto<LookupDto<Guid>>> GetTherapistLookupAsync()
        {
            var therapists = await Repository.GetListAsync();
            return new ListResultDto<LookupDto<Guid>>(
                ObjectMapper.Map<List<Therapist>, List<LookupDto<Guid>>>(therapists)
            );
        }
    }
}