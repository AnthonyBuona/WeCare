
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WeCare.Application.Contracts.Consultations;
using WeCare.Shared; 

namespace WeCare.Objectives
{
    public interface IObjectiveAppService : ICrudAppService<
        ObjectiveDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateObjectiveDto>
    {
        Task<ListResultDto<LookupDto<Guid>>> GetObjectiveLookupAsync(Guid patientId);
        Task<List<ObjectiveGroupDto>> GetGroupedObjectivesByPatientAsync(Guid patientId);
    }
}