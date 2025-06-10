using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using WeCare.Permissions;
using WeCare.Shared;

// Apelidos para resolver ambiguidade de namespaces
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using IdentityRole = Volo.Abp.Identity.IdentityRole;
using Volo.Abp;

namespace WeCare.Therapists
{
    // Vamos herdar de CrudAppService para ganhar os métodos de Leitura, Update e Delete de graça.
    public class TherapistAppService : CrudAppService<
            Therapist,
            TherapistDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateTherapistDto>,
        ITherapistAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TherapistAppService(
            IRepository<Therapist, Guid> repository,
            IdentityUserManager userManager,
            RoleManager<IdentityRole> roleManager)
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;

            // Definindo as permissões para as operações do CRUD
            GetPolicyName = WeCarePermissions.Therapists.Default;
            GetListPolicyName = WeCarePermissions.Therapists.Default;
            CreatePolicyName = WeCarePermissions.Therapists.Create;
            UpdatePolicyName = WeCarePermissions.Therapists.Edit;
            DeletePolicyName = WeCarePermissions.Therapists.Delete;
        }

        // Sobrescrevemos apenas o CreateAsync porque ele tem uma lógica customizada.
        [Authorize(WeCarePermissions.Therapists.Create)]
        public override async Task<TherapistDto> CreateAsync(CreateUpdateTherapistDto input)
        {
            var existingUser = await _userManager.FindByNameAsync(input.UserName);
            if (existingUser != null)
            {
                throw new UserFriendlyException("Já existe um usuário com este nome.");
            }

            var user = new IdentityUser(GuidGenerator.Create(), input.UserName, input.Email);
            (await _userManager.CreateAsync(user, input.Password)).CheckErrors();

            const string therapistRole = "Terapeuta";
            if (!await _roleManager.RoleExistsAsync(therapistRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(GuidGenerator.Create(), therapistRole, CurrentTenant.Id));
            }
            (await _userManager.AddToRoleAsync(user, therapistRole)).CheckErrors();

            var therapist = new Therapist
            {
                Name = input.Name,
                Email = input.Email,
                UserId = user.Id
            };

            await Repository.InsertAsync(therapist);

            return ObjectMapper.Map<Therapist, TherapistDto>(therapist);
        }

        // Esta é a nossa função customizada para o dropdown, ela funciona corretamente.
        public async Task<ListResultDto<LookupDto<Guid>>> GetTherapistLookupAsync()
        {
            var therapists = await Repository.GetListAsync();
            return new ListResultDto<LookupDto<Guid>>(
                ObjectMapper.Map<List<Therapist>, List<LookupDto<Guid>>>(therapists)
            );
        }
    }
}