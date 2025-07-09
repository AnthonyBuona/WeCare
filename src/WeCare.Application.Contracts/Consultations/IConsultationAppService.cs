using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WeCare.Consultations
{
    public interface IConsultationAppService :
        ICrudAppService<
            ConsultationDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateConsultationDto>
    {

    }
}