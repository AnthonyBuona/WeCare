using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WeCare.Shared;

namespace WeCare.PeriodicReports
{
    public interface IPeriodicReportAppService : ICrudAppService<
        PeriodicReportDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdatePeriodicReportDto>
    {
        Task<PeriodicReportDto> GetDraftReportAsync(Guid patientId, DateTime startDate, DateTime endDate);
        Task<PeriodicReportDto> SignAndPublishAsync(Guid id);
        Task<PeriodicReportDto> ParentSignAsync(Guid id, ParentSignReportDto input);
        Task<List<LookupDto<Guid>>> GetPatientLookupAsync();
        Task<List<LookupDto<Guid>>> GetTherapistLookupAsync();
    }
}
