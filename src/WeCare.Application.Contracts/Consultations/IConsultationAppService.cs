using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WeCare.Consultations;

namespace WeCare.Application.Contracts.Consultations
{
    public interface IConsultationAppService : ICrudAppService<
        ConsultationDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateConsultationDto>
    {
        Task CreateObjectiveAsync(CreateUpdateObjectiveDto input);
        Task<List<ObjectiveGroupDto>> GetGroupedByPatientAsync(Guid patientId);
        Task<ListResultDto<string>> GetObjectiveNamesForPatientAsync(Guid patientId);
    }
}