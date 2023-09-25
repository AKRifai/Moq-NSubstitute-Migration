using Microsoft.Extensions.Logging;
using NotificationSystem.Interfaces;
using NotificationSystem.Models;

namespace NotificationSystem.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger _logger;
        public NotificationService(IEmailService emailService, ILogger logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public bool NotifyUser(User user, string message)
        {
            try
            {
                if (!_emailService.IsValidEmail(user.Email) ||
                    string.IsNullOrWhiteSpace(message))
                {
                    _logger.LogError("Email is not valid");
                    return false;
                }

                return _emailService.SendEmail(user.Email, "Notification", message);
            }
            catch (Exception)
            {
                _logger.LogError($"Error sending email to user: {user.Email}");
                return false;
            }
        }
    }
}
