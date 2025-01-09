using augalinga.Backend.Services;
using augalinga.Data.Access;
using augalinga.Data.Entities;
using augalinga.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace augalinga.Tests
{
    public class NotificationServiceTests
    {
        private Mock<IAuthService> CreateAuthServiceMock(User currentUser)
        {
            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(s => s.GetCurrentUser()).Returns(currentUser);
            return authServiceMock;
        }

        private DataContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new DataContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public void CreateNotification_ShouldAddNotification_WhenCalled()
        {
            // Arrange
            var context = CreateDbContext();
            var currentUser = new User { Id = 1, FullName = "Test User" };
            var authServiceMock = CreateAuthServiceMock(currentUser);

            var notificationService = new NotificationService(authServiceMock.Object, context);

            // Act
            notificationService.CreateNotification("Test Entity", "Test Project", NotificationType.ProjectAdded, null);

            // Assert
            Assert.Single(context.Notifications);
            var notification = context.Notifications.First();
            Assert.Equal("Project - Added - Test Entity", notification.Message);
            Assert.Equal(currentUser.Id, notification.UserId);
            Assert.Null(notification.ForUserId);
            Assert.Equal(NotificationType.ProjectAdded, notification.Type);
        }

        [Fact]
        public void CreateNotification_ShouldThrow_WhenCurrentUserIsNull()
        {
            // Arrange
            var context = CreateDbContext();
            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(s => s.GetCurrentUser()).Returns((User)null);

            var notificationService = new NotificationService(authServiceMock.Object, context);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                notificationService.CreateNotification("Test Entity", "Test Project", NotificationType.ProjectAdded, null));
        }

        [Fact]
        public void AddNotification_ShouldAddToObservableCollectionAndDatabase()
        {
            // Arrange
            var context = CreateDbContext();
            var currentUser = new User { Id = 1, FullName = "Test User" };
            var authServiceMock = CreateAuthServiceMock(currentUser);

            var notificationService = new NotificationService(authServiceMock.Object, context);
            var notification = new Notification
            {
                Message = "Test Notification",
                CreatedAt = DateTime.UtcNow,
                UserId = currentUser.Id,
                Type = NotificationType.ProjectAdded
            };

            // Act
            notificationService.CreateNotification("Test Entity", "Test Project", NotificationType.ProjectAdded, null);

            // Assert
            Assert.Single(notificationService.Notifications);
            Assert.Single(context.Notifications);
            Assert.Equal("Project - Added - Test Entity", notificationService.Notifications[0].Message);
        }

    }
}
