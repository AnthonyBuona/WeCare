using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WeCare.Permissions;
using WeCare.Responsibles;
using WeCare.Patients;

namespace WeCare.Guests
{
    public class GuestAppService : WeCareAppService, IGuestAppService
    {
        private readonly IRepository<Guest, Guid> _guestRepository;
        private readonly IRepository<Responsible, Guid> _responsibleRepository;
        private readonly IRepository<Patient, Guid> _patientRepository;

        public GuestAppService(
            IRepository<Guest, Guid> guestRepository,
            IRepository<Responsible, Guid> responsibleRepository,
            IRepository<Patient, Guid> patientRepository)
        {
            _guestRepository = guestRepository;
            _responsibleRepository = responsibleRepository;
            _patientRepository = patientRepository;
        }

        [Authorize(WeCarePermissions.Guests.Default)]
        public async Task<GuestDto> GetAsync(Guid id)
        {
            var guest = await _guestRepository.GetAsync(id);
             if (CurrentUser.Id != null)
             {
                 // Check if user is the responsible for this guest
                  var responsible = await _responsibleRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);
                  
                   // TODO: In a real scenario, check if guest belongs to user. 
                   // For now simplified.
                  if (responsible != null && guest.ResponsibleId != responsible.Id)
                  {
                      // Allow admin
                       if (!CurrentUser.IsInRole("admin") && !CurrentUser.IsInRole("WeCareDev"))
                         throw new UserFriendlyException("Acesso negado.");
                  }
             }
            return ObjectMapper.Map<Guest, GuestDto>(guest);
        }

        [Authorize(WeCarePermissions.Guests.Edit)]
        public async Task<GuestDto> UpdateAsync(Guid id, CreateGuestDto input)
        {
            var guest = await _guestRepository.GetAsync(id);

             if (CurrentUser.Id != null)
            {
                 var responsible = await _responsibleRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);
                 if (responsible != null && guest.ResponsibleId != responsible.Id)
                 {
                      if (!CurrentUser.IsInRole("admin") && !CurrentUser.IsInRole("WeCareDev"))
                         throw new UserFriendlyException("Acesso negado.");
                 }
            }
            
            guest.Name = input.Name;
            guest.Email = input.Email;
            guest.PatientId = input.PatientId;
            // ResponsibleId not changed usually

            await _guestRepository.UpdateAsync(guest);
            return ObjectMapper.Map<Guest, GuestDto>(guest);
        }

        [Authorize(WeCarePermissions.Guests.Create)]
        public async Task<GuestDto> CreateAsync(CreateGuestDto input)
        {
            if (CurrentUser.Id == null)
            {
                throw new UserFriendlyException("Usuário não logado.");
            }

            // Tenta encontrar o Responsible pelo UserId
            var responsible = await _responsibleRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);

            // Fallback para email se não encontrar
            if (responsible == null && CurrentUser.Email != null)
            {
                responsible = await _responsibleRepository.FirstOrDefaultAsync(x => x.EmailAddress == CurrentUser.Email);
                if (responsible != null)
                {
                    responsible.UserId = CurrentUser.Id;
                    await _responsibleRepository.UpdateAsync(responsible, autoSave: true);
                }
            }

            if (responsible == null)
            {
                throw new UserFriendlyException("Você precisar ser um Responsável para convidar convidados.");
            }

            // Verifica vínculo com o paciente
            var patient = await _patientRepository.GetAsync(input.PatientId);
            if (patient == null)
            {
                 throw new UserFriendlyException("Paciente não encontrado.");
            }
            
            if (patient.PrincipalResponsibleId != responsible.Id)
            {
                throw new UserFriendlyException("Você não tem permissão para adicionar convidados para este paciente.");
            }

            var guest = new Guest
            {
                ResponsibleId = responsible.Id,
                PatientId = input.PatientId,
                Name = input.Name,
                Email = input.Email,
                TenantId = CurrentTenant.Id
            };

            await _guestRepository.InsertAsync(guest);

            return ObjectMapper.Map<Guest, GuestDto>(guest);
        }

        [Authorize(WeCarePermissions.Guests.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            // Validação simples: verificar se o Guest existe (ABP já faria isso no Get)
            // Validação de segurança: garantir que só o dono pode apagar
            
            var guest = await _guestRepository.GetAsync(id);
            
            if (CurrentUser.Id != null)
            {
                 var responsible = await _responsibleRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);
                 if (responsible != null && guest.ResponsibleId != responsible.Id)
                 {
                     // Se for admin (WeCareDev), talvez possa deletar? Por enquanto restrito ao dono.
                     // Mas verifique role admin
                     if (!CurrentUser.IsInRole("admin") && !CurrentUser.IsInRole("WeCareDev"))
                     {
                        throw new UserFriendlyException("Você não tem permissão para remover este convidado.");
                     }
                 }
            }

             await _guestRepository.DeleteAsync(id);
        }

        [Authorize(WeCarePermissions.Guests.Default)]
        public async Task<PagedResultDto<GuestDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            if (CurrentUser.Id == null) return new PagedResultDto<GuestDto>();
            
             var responsible = await _responsibleRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);
             
             // Tentativa de auto-link se necessário
             if (responsible == null && CurrentUser.Email != null)
             {
                responsible = await _responsibleRepository.FirstOrDefaultAsync(x => x.EmailAddress == CurrentUser.Email);
                if (responsible != null)
                {
                     responsible.UserId = CurrentUser.Id;
                     await _responsibleRepository.UpdateAsync(responsible);
                }
             }

             if (responsible == null) return new PagedResultDto<GuestDto>();

            var query = await _guestRepository.GetQueryableAsync();
            
            // Filtra guests criados pelo responsável logado
            query = query.Where(x => x.ResponsibleId == responsible.Id);

            var totalCount = await AsyncExecuter.CountAsync(query);
            
            query = query.OrderBy(input.Sorting ?? "Name");
            query = query.PageBy(input);

            var guests = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<GuestDto>(
                totalCount,
                ObjectMapper.Map<List<Guest>, List<GuestDto>>(guests)
            );
        }

        [Authorize(WeCarePermissions.Guests.Default)]
        public async Task<ListResultDto<WeCare.Shared.LookupDto<Guid>>> GetMyPatientsLookupAsync()
        {
             if (CurrentUser.Id == null) return new ListResultDto<WeCare.Shared.LookupDto<Guid>>();
            
             var responsible = await _responsibleRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);
             
             if (responsible == null && CurrentUser.Email != null)
                responsible = await _responsibleRepository.FirstOrDefaultAsync(x => x.EmailAddress == CurrentUser.Email);

             var query = await _patientRepository.GetQueryableAsync();

             // Se for admin ou WeCareDev, vê todos. Se não, filtra.
             if (!CurrentUser.IsInRole("admin") && !CurrentUser.IsInRole("WeCareDev")) 
             {
                 if (responsible == null) return new ListResultDto<WeCare.Shared.LookupDto<Guid>>();
                 query = query.Where(x => x.PrincipalResponsibleId == responsible.Id);
             }

             var patients = await AsyncExecuter.ToListAsync(query);
             
             return new ListResultDto<WeCare.Shared.LookupDto<Guid>>(
                 patients.Select(x => new WeCare.Shared.LookupDto<Guid> { Id = x.Id, DisplayName = x.Name }).ToList()
             );
        }
    }
}
