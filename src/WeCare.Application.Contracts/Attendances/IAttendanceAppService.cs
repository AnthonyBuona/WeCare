using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WeCare.Shared;

namespace WeCare.Attendances
{
    public interface IAttendanceAppService : ICrudAppService<
        AttendanceDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateAttendanceDto>
    {
        Task<List<LookupDto<Guid>>> GetPatientLookupAsync();
    }
}
