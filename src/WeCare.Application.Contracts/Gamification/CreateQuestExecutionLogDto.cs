using System;
using System.ComponentModel.DataAnnotations;

namespace WeCare.Gamification
{
    public class CreateQuestExecutionLogDto
    {
        [Required]
        public Guid QuestId { get; set; }

        [Required]
        [Range(1, 5)]
        public int EnjoymentScore { get; set; }

        [StringLength(1000)]
        public string CaregiverNotes { get; set; }
    }
}
