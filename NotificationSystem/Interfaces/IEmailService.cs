namespace NotificationSystem.Interfaces
{
    public interface IEmailService
    {
        bool IsValidEmail(string email);
        bool SendEmail(string recipent, string subject, string body);
    }
}
