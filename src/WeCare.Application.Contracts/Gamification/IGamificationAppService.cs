using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WeCare.Gamification
{
    public interface IGamificationAppService : IApplicationService
    {
        Task<CaregiverQuestDto> CreateQuestAsync(Guid patientId, string title, string instructions, string videoTutorialUrl, int xpReward);
        
        Task<List<CaregiverQuestDto>> GetQuestsByPatientAsync(Guid patientId);
        
        Task<QuestExecutionLogDto> ExecuteQuestAsync(CreateQuestExecutionLogDto input);
        
        Task<UserGamifiedProfileDto> GetProfileAsync(Guid parentUserId);
        
        Task<int> GetActiveStreakAsync(Guid parentUserId);
        
        Task<int> IncrementStreakAsync(Guid parentUserId);
    }
}
