using NotificationSystem.Models;

namespace NotificationSystem.Interfaces
{
    public interface INotificationService
    {
        bool NotifyUser(User user, string message);
    }
}
