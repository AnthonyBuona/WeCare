using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WeCare.Clinics
{
    public interface IClinicAppService : ICrudAppService<
        ClinicDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateClinicDto,
        CreateUpdateClinicDto>
    {
        Task<ClinicDto> ChangeStatusAsync(Guid id, ChangeClinicStatusDto input);
        Task<ClinicDto> FreezeAsync(Guid id);
        Task<ClinicDto> ActivateAsync(Guid id);

        Task<ClinicSettingsDto> GetSettingsAsync(Guid id);
        Task<ClinicSettingsDto> GetCurrentClinicSettingsAsync();
        Task UpdateSettingsAsync(Guid id, ClinicSettingsDto input);
        Task UpdateCurrentClinicSettingsAsync(ClinicSettingsDto input);
    }
}
