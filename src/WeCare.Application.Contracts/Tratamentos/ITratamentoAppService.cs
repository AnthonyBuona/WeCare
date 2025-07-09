using System;
using System.Threading.Tasks; 
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WeCare.Tratamentos
{
    public interface ITratamentoAppService :
        ICrudAppService<
            TratamentoDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateTratamentoDto>
    {
        // Adicione esta linha
        Task<PagedResultDto<TratamentoDto>> GetListByPatient(Guid patientId, PagedAndSortedResultRequestDto input);
    }
}