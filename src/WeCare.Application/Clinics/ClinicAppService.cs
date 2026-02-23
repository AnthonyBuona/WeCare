using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace WeCare.Clinics
{
    public class ClinicAppService : CrudAppService<
        Clinic,
        ClinicDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateClinicDto,
        CreateUpdateClinicDto>, IClinicAppService
    {
        public ClinicAppService(IRepository<Clinic, Guid> repository)
            : base(repository)
        {
        }

        public async Task<ClinicDto> ChangeStatusAsync(Guid id, ChangeClinicStatusDto input)
        {
            var clinic = await Repository.GetAsync(id);
            clinic.Status = input.Status;
            await Repository.UpdateAsync(clinic);
            return await MapToGetOutputDtoAsync(clinic);
        }

        public async Task<ClinicDto> FreezeAsync(Guid id)
        {
            return await ChangeStatusAsync(id, new ChangeClinicStatusDto
            {
                Status = ClinicStatus.Frozen
            });
        }

        public async Task<ClinicDto> ActivateAsync(Guid id)
        {
            return await ChangeStatusAsync(id, new ChangeClinicStatusDto
            {
                Status = ClinicStatus.Active
            });
        }

        public async Task<ClinicSettingsDto> GetSettingsAsync(Guid id)
        {
            var query = await Repository.WithDetailsAsync(x => x.OperatingHours);
            var clinic = await AsyncExecuter.FirstOrDefaultAsync(query, x => x.Id == id);
            
            if (clinic == null) throw new Volo.Abp.UserFriendlyException("Clínica não encontrada.");

            return ObjectMapper.Map<Clinic, ClinicSettingsDto>(clinic);
        }

        public async Task<ClinicSettingsDto> GetCurrentClinicSettingsAsync()
        {
            if (CurrentTenant.Id == null) return null;

            var query = await Repository.WithDetailsAsync(x => x.OperatingHours);
            var clinic = await AsyncExecuter.FirstOrDefaultAsync(query, x => x.TenantId == CurrentTenant.Id);
            
            if (clinic == null) return null;

            if (clinic == null) return null;

            return ObjectMapper.Map<Clinic, ClinicSettingsDto>(clinic);
        }

        public async Task UpdateCurrentClinicSettingsAsync(ClinicSettingsDto input)
        {
            if (CurrentTenant.Id == null) throw new Volo.Abp.UserFriendlyException("Tenant não identificado.");

            var clinic = await Repository.FirstOrDefaultAsync(x => x.TenantId == CurrentTenant.Id);
            
            if (clinic == null)
            {
                // Auto-create clinic for this tenant on first settings save
                clinic = new Clinic
                {
                    TenantId = CurrentTenant.Id,
                    Name = CurrentTenant.Name ?? "Clínica"
                };
                clinic = await Repository.InsertAsync(clinic, autoSave: true);
            }

            await UpdateSettingsAsync(clinic.Id, input);
        }

        public async Task UpdateSettingsAsync(Guid id, ClinicSettingsDto input)
        {
            var query = await Repository.WithDetailsAsync(x => x.OperatingHours);
            var clinic = await AsyncExecuter.FirstOrDefaultAsync(query, x => x.Id == id);

            if (clinic == null) throw new Volo.Abp.UserFriendlyException("Clínica não encontrada.");

            // White-Label
            clinic.LogoUrl = input.LogoUrl;
            clinic.PrimaryColor = input.PrimaryColor;
            clinic.SecondaryColor = input.SecondaryColor;

            // Address & Contact
            clinic.Address = input.Address;
            clinic.Phone = input.Phone;
            clinic.Email = input.Email;
            clinic.AddressNumber = input.AddressNumber;
            clinic.Neighborhood = input.Neighborhood;
            clinic.City = input.City;
            clinic.State = input.State;
            clinic.ZipCode = input.ZipCode;
            clinic.WebsiteUrl = input.WebsiteUrl;
            clinic.InstagramUrl = input.InstagramUrl;
            clinic.FacebookUrl = input.FacebookUrl;
            clinic.LinkedInUrl = input.LinkedInUrl;
            clinic.WelcomeMessage = input.WelcomeMessage;

            // Scheduling
            clinic.AppointmentDurationMinutes = input.AppointmentDurationMinutes;
            clinic.Specializations = input.Specializations;

            // Update Operating Hours (Full Replace Strategy)
            clinic.OperatingHours.Clear();
            foreach (var item in input.OperatingHours)
            {
                clinic.OperatingHours.Add(new ClinicOperatingHour(
                    GuidGenerator.Create(),
                    clinic.Id,
                    item.DayOfWeek,
                    item.StartTime,
                    item.EndTime,
                    item.IsClosed
                ) {
                    BreakStart = item.BreakStart,
                    BreakEnd = item.BreakEnd
                });
            }

            await Repository.UpdateAsync(clinic);
        }
    }
}
