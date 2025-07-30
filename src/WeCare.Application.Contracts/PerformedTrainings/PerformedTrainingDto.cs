using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Application.Contracts.PerformedTrainings
{
    public class PerformedTrainingDto : EntityDto<Guid>
    {
        public string Activity { get; set; }
        public string HelpNeeded { get; set; }
        public int TotalAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
    }
}