using System;

namespace WeCare.Notifications
{
    public class NotificationDto
    {
        public string Id { get; set; }
        public string Type { get; set; } // "consultation", "report", "consent"
        public string Title { get; set; }
        public string Message { get; set; }
        public string ActionUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
