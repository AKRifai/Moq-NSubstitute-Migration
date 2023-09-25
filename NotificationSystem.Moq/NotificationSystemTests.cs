using Moq;
using NotificationSystem.Interfaces;
using NotificationSystem.Models;
using NotificationSystem.Services;
using NUnit.Framework.Internal;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace NotificationSystem.Moq
{
    public class NotificationSystemTests
    {
        private Mock<IEmailService> _emailServiceMock;
        private Mock<ILogger> _loggerMock;
        private NotificationService _notificationService;

        [SetUp]
        public void Setup()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILogger>();
            _notificationService = new NotificationService(_emailServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public void NotifyUser_ValidEmailAndMessage_ReturnsTrue()
        {
            // Arrange
            _emailServiceMock.Setup(s => s.IsValidEmail(It.IsAny<string>())).Returns(true);
            _emailServiceMock.Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var user = new User { Email = "test@example.com" };
            const string message = "Test message";

            // Act
            var result = _notificationService.NotifyUser(user, message);

            // Assert
            Assert.IsTrue(result);
            _emailServiceMock.Verify(s => s.IsValidEmail(user.Email), Times.Once);
            _emailServiceMock.Verify(s => s.SendEmail(user.Email, "Notification", message), Times.Once);
        }

        [Test]
        public void NotifyUser_InvalidEmail_ReturnsFalse()
        {
            // Arrange
            _emailServiceMock.Setup(s => s.IsValidEmail(It.IsAny<string>())).Returns(false);

            var user = new User { Email = "invalid email" };
            const string message = "Test message";

            // Act
            var result = _notificationService.NotifyUser(user, message);

            // Assert
            Assert.IsFalse(result);
            _emailServiceMock.Verify(s => s.IsValidEmail(user.Email), Times.Once);
            _emailServiceMock.Verify(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _loggerMock.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
                            It.IsAny<EventId>(),
                            It.Is<It.IsAnyType>((v, t) => true),
                            It.IsAny<Exception>(),
                            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Test]
        public void NotifyUser_SendEmailThrowsException_ReturnsFalse()
        {
            // Arrange
            _emailServiceMock.Setup(s => s.IsValidEmail(It.IsAny<string>())).Returns(true);
            _emailServiceMock.Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws<Exception>();

            var user = new User { Email = "test@example.com" };
            const string message = "Test message";

            // Act
            var result = _notificationService.NotifyUser(user, message);

            // Assert
            Assert.IsFalse(result);
            _emailServiceMock.Verify(s => s.IsValidEmail(user.Email), Times.Once);
            _emailServiceMock.Verify(s => s.SendEmail(user.Email, "Notification", message), Times.Once);
        }
    }
}