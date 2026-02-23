using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WeCare.Shared;

namespace WeCare.Therapists
{
    // Interface para o lookup, pode ser usada separadamente se necessário
    public interface ITherapistLookupAppService : IApplicationService
    {
        Task<ListResultDto<LookupDto<Guid>>> GetTherapistLookupAsync();
    }

    // Interface principal do CRUD que inclui o lookup
    public interface ITherapistAppService :
        ICrudAppService<
            TherapistDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateTherapistDto>,
        ITherapistLookupAppService
    {
        Task<ListResultDto<LookupDto<Guid>>> GetTherapistsByPatientAsync(Guid patientId);
    }
}