using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WeCare.Guests;
using WeCare.Shared;

namespace WeCare.Guests
{
    public interface IGuestAppService : ICrudAppService<
        GuestDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateGuestDto>
    {
        Task<ListResultDto<LookupDto<Guid>>> GetPatientLookupAsync();
        Task<ListResultDto<LookupDto<Guid>>> GetResponsibleLookupAsync();
    }
}
