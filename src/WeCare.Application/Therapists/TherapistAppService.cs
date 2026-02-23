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
        private readonly IRepository<Tratamentos.Tratamento, Guid> _tratamentoRepository;

        public TherapistAppService(
            IRepository<Therapist, Guid> repository,
            IdentityUserManager userManager,
            IIdentityRoleRepository roleRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Tratamentos.Tratamento, Guid> tratamentoRepository)
            : base(repository)
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _tratamentoRepository = tratamentoRepository;

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
                throw new UserFriendlyException("Já existe um usuário com este nome de usuário.");
            }

            var existingEmailUser = await _userManager.FindByEmailAsync(input.Email);
            if (existingEmailUser != null)
            {
                throw new UserFriendlyException("Este e-mail já está em uso por outro usuário.");
            }

            // 1. Criar o usuário de identidade.
            var user = new Volo.Abp.Identity.IdentityUser(GuidGenerator.Create(), input.UserName, input.Email)
            {
                Name = input.Name
            };
            (await _userManager.CreateAsync(user, input.Password)).CheckErrors();

            // 2. Atribuir a role "Terapeuta".
            const string therapistRole = "Therapist";
            (await _userManager.AddToRoleAsync(user, therapistRole)).CheckErrors();

            // 3. Salvar as mudanças no Identity (Usuário e Role)
            if (_unitOfWorkManager.Current != null)
            {
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }

            // 4. Criar explicitamente a entidade Therapist (Evitando race conditions de EventHandlers)
            var newTherapist = new Therapist
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Specialization = input.Specialization
            };

            await Repository.InsertAsync(newTherapist, autoSave: true);

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

        public async Task<ListResultDto<LookupDto<Guid>>> GetTherapistsByPatientAsync(Guid patientId)
        {
            var tratamentos = await _tratamentoRepository.GetListAsync(t => t.PatientId == patientId);
            var therapistIds = tratamentos.Select(t => t.TherapistId).Distinct().ToList();

            if (!therapistIds.Any())
            {
                return new ListResultDto<LookupDto<Guid>>();
            }

            var therapists = await Repository.GetListAsync(t => therapistIds.Contains(t.Id));
            return new ListResultDto<LookupDto<Guid>>(
                ObjectMapper.Map<List<Therapist>, List<LookupDto<Guid>>>(therapists)
            );
        }
    }
}