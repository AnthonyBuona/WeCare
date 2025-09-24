using System;
using Volo.Abp.Application.Dtos;
using WeCare.Domain.Shared.PerformedTrainings;

namespace WeCare.Application.Contracts.PerformedTrainings
{
    public class PerformedTrainingDto : EntityDto<Guid>
    {
        public Guid TrainingId { get; set; }
        public string TrainingName { get; set; }
        public HelpNeededType HelpNeeded { get; set; }
        public int TotalAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
    }
}