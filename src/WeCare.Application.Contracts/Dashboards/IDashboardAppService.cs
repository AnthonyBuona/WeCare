using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Services;

namespace WeCare.Dashboards
{
    public interface IDashboardAppService : IApplicationService
    {
        Task<WeCareDashboardHeaderStatsDto> GetStatsAsync();
        Task<PatientDashboardDto> GetPatientDashboardAsync(Guid patientId);
    }
}
