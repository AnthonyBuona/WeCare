using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WeCare.Shared;

namespace WeCare.Patients;

public interface IPatientLookupAppService : IApplicationService
{
    Task<ListResultDto<LookupDto<Guid>>> GetPatientLookupAsync();
}

public interface IPatientAppService :
    ICrudAppService<
        PatientDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdatePatientDto>,
       IPatientLookupAppService 
{
 
}