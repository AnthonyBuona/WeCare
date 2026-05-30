using System;
using Volo.Abp.Domain.Repositories;

namespace WeCare.PeriodicReports
{
    public interface IPeriodicReportRepository : IRepository<PeriodicReport, Guid>
    {
    }
}
