using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WeCare.Clinics;

public interface IClinicManagementAppService : IApplicationService
{
    Task<ClinicDto> CreateAsync(CreateClinicInput input);
    Task<PagedResultDto<ClinicDto>> GetListAsync(PagedAndSortedResultRequestDto input);
}
