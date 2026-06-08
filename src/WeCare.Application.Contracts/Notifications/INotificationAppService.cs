using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WeCare.Notifications
{
    public interface INotificationAppService : IApplicationService
    {
        Task<List<NotificationDto>> GetUnreadAsync();
        Task MarkAsReadAsync(string notificationId);
    }
}
