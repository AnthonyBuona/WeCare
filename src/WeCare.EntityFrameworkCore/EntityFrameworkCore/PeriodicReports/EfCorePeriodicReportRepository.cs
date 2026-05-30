using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WeCare.PeriodicReports;

namespace WeCare.EntityFrameworkCore.PeriodicReports
{
    public class EfCorePeriodicReportRepository : EfCoreRepository<WeCareDbContext, PeriodicReport, Guid>, IPeriodicReportRepository
    {
        public EfCorePeriodicReportRepository(IDbContextProvider<WeCareDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
