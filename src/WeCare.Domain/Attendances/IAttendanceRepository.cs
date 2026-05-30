using System;
using Volo.Abp.Domain.Repositories;

namespace WeCare.Attendances
{
    public interface IAttendanceRepository : IRepository<Attendance, Guid>
    {
    }
}
