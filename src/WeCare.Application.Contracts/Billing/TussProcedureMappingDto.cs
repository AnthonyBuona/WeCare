using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Billing
{
    public class TussProcedureMappingDto : EntityDto<Guid>
    {
        public string Specialty { get; set; }
        public string TussCode { get; set; }
        public string Description { get; set; }
    }
}
