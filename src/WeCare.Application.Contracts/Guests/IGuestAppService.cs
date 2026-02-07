using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WeCare.Guests
{
    public interface IGuestAppService : IApplicationService
    {
        Task<GuestDto> GetAsync(Guid id);
        Task<GuestDto> CreateAsync(CreateGuestDto input);
        Task<GuestDto> UpdateAsync(Guid id, CreateGuestDto input);
        Task DeleteAsync(Guid id);
        Task<PagedResultDto<GuestDto>> GetListAsync(PagedAndSortedResultRequestDto input);
        Task<Volo.Abp.Application.Dtos.ListResultDto<WeCare.Shared.LookupDto<Guid>>> GetMyPatientsLookupAsync();
    }
}
