using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace WeCare.Clinics
{
    public class ClinicAppService : CrudAppService<
        Clinic,
        ClinicDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateClinicDto,
        CreateUpdateClinicDto>, IClinicAppService
    {
        public ClinicAppService(IRepository<Clinic, Guid> repository)
            : base(repository)
        {
        }

        public async Task<ClinicDto> ChangeStatusAsync(Guid id, ChangeClinicStatusDto input)
        {
            var clinic = await Repository.GetAsync(id);
            clinic.Status = input.Status;
            await Repository.UpdateAsync(clinic);
            return await MapToGetOutputDtoAsync(clinic);
        }

        public async Task<ClinicDto> FreezeAsync(Guid id)
        {
            return await ChangeStatusAsync(id, new ChangeClinicStatusDto
            {
                Status = ClinicStatus.Frozen
            });
        }

        public async Task<ClinicDto> ActivateAsync(Guid id)
        {
            return await ChangeStatusAsync(id, new ChangeClinicStatusDto
            {
                Status = ClinicStatus.Active
            });
        }
    }
}
