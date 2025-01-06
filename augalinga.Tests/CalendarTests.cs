using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using augalinga.Data.Entities;
using augalinga.Data.Enums;
using augalinga.Backend.ViewModels;
using augalinga.Backend.Services;
using augalinga.Data.Access;

namespace augalinga.Tests
{
    public class CalendarTests
    {
        private readonly Mock<INotificationService> _mockNotificationsService;
        private readonly Mock<IAuthService> _mockAuthService;

        public CalendarTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockNotificationsService = new Mock<INotificationService>();
            _mockNotificationsService
        .Setup(n => n.CreateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>(), It.IsAny<int?>()))
        .Callback((string entityName, string? projectName, NotificationType type, int? forUserId) =>
        {
            Console.WriteLine($"Notification Created: {entityName}, {type}, UserId: {forUserId}");
        })
        .Verifiable();
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

        public CalendarViewModel CreateCalendarViewModel(DataContext dbContext)
        {
            return new CalendarViewModel(_mockNotificationsService.Object, dbContext);
        }

        [Fact]
        public void ToggleAssignToAllUsers_ShouldAssignAllUsers_WhenAssignedToAll()
        {
            var context = CreateDbContext();
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, FullName = "User 1" },
                new User { Id = 2, FullName = "User 2" },
                new User { Id = 3, FullName = "User 3" }
            };

            var meeting = new Meeting
            {
                IsAssignedToAllUsers = true,
                SelectedUsers = new List<User>()  // Initially empty
            };

            var viewModel = new CalendarViewModel(_mockNotificationsService.Object, context)
            {
                Users = users // Assign users to the ViewModel
            };

            // Act
            viewModel.ToggleAssignToAllUsers(meeting);

            // Assert
            Assert.Equal(3, meeting.SelectedUsers.Count); 
            Assert.Contains(meeting.SelectedUsers, u => u.Id == 1);
            Assert.Contains(meeting.SelectedUsers, u => u.Id == 2);
            Assert.Contains(meeting.SelectedUsers, u => u.Id == 3);
        }

        [Fact]
        public void ToggleAssignToAllUsers_ShouldClearSelectedUsers_WhenNotAssignedToAll()
        {
            var context = CreateDbContext();
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, FullName = "User 1" },
                new User { Id = 2, FullName = "User 2" }
            };

            var meeting = new Meeting
            {
                IsAssignedToAllUsers = false,
                SelectedUsers = new List<User> { users[0] } 
            };

            var viewModel = new CalendarViewModel(_mockNotificationsService.Object, context)
            {
                Users = users
            };

            // Act
            viewModel.ToggleAssignToAllUsers(meeting);

            // Assert
            Assert.Empty(meeting.SelectedUsers);
        }




        [Fact]
        public async Task LoadEvents_ShouldLoadAllEvents_WhenNoUserIdsSelected()
        {
            // Arrange
            using var context = CreateDbContext();
            context.Meetings.AddRange(new List<Meeting>
            {
                new Meeting { Id = 1, EventName = "Meeting 1", BackgroundColor = "#12343" },
                new Meeting { Id = 2, EventName = "Meeting 2", BackgroundColor = "#12343" }
            });
            await context.SaveChangesAsync();
            foreach (var meeting in context.Meetings)
            {
                Console.WriteLine(meeting.EventName);
            }

            var viewModel = CreateCalendarViewModel(context);
            viewModel.Initialize();

            // Act
            viewModel.LoadEvents(new List<int>());

            // Assert
            Assert.Equal(2, viewModel.Events.Count); // Two events should be loaded
            Assert.Contains(viewModel.Events, e => e.EventName == "Meeting 1");
            Assert.Contains(viewModel.Events, e => e.EventName == "Meeting 2");
        }
        [Fact]
        public async Task LoadEvents_ShouldFilterEventsByUserIds()
        {
            // Arrange
            var context = CreateDbContext();
            var user1 = new User { Id = 1, FullName = "User 1", Email = "user1@example.com", Password = "password", Token = "token1", Color = "#FF0000" };
            var user2 = new User { Id = 2, FullName = "User 2", Email = "user2@example.com", Password = "password", Token = "token2", Color = "#00FF00" };
            context.Users.AddRange(user1, user2);
            context.Meetings.AddRange(new List<Meeting>
            {
                new Meeting { Id = 1, EventName = "Meeting 1", BackgroundColor = "#FF0000", SelectedUsers = new List<User> { user1 } },
                new Meeting { Id = 2, EventName = "Meeting 2", BackgroundColor = "#00FF00", SelectedUsers = new List<User> { user2 } }
            });
            context.SaveChanges();

            var viewModel = CreateCalendarViewModel(context);
            viewModel.Initialize();

            // Act
            viewModel.LoadEvents(new List<int> { 1 });

            // Assert
            Assert.Single(viewModel.Events);
        }

        [Fact]
        public async Task CreateEvent_ShouldAddMeetingAndTriggerNotifications()
        {
            // Arrange
            var context = CreateDbContext();
            var users = new List<User>
            {
                new User { Id = 1, FullName = "Peter", Color = "#FF0000", Email = "user1@example.com", Password = "password", Token = "token1" },
                new User { Id = 2, FullName = "Bob", Color = "#00FF00", Email = "user2@example.com", Password = "password", Token = "token2" }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            var newMeeting = new Meeting
            {
                EventName = "New Meeting",
                SelectedUsers = new List<User> { users[0] },
                From = DateTime.Now,
                To = DateTime.Now.AddHours(1),
                BackgroundColor = "234"
            };

            var viewModel = CreateCalendarViewModel(context);
            viewModel.Initialize();

            // Act
            await viewModel.CreateEvent(newMeeting);

                // Assert
                Assert.Single(viewModel.Events);
                Assert.Contains(viewModel.Events, e => e.EventName == "New Meeting");

                _mockNotificationsService.Verify(n =>
                    n.CreateNotification("New Meeting", null, NotificationType.MeetingAdded, 1), Times.Once);
        }

        [Fact]
        public async Task DeleteEvent_ShouldRemoveMeeting()
        {
            // Arrange
            var context = CreateDbContext();
            var user = new User { Id = 1, FullName = "Peter", Color = "#FF0000", Email = "user1@example.com", Password = "password", Token = "token1" };
            var meeting = new Meeting
            {
                Id = 1,
                EventName = "Meeting to Delete",
                SelectedUsers = new List<User> { user },
                BackgroundColor = "234"
            };

            context.Users.Add(user);
            context.Meetings.Add(meeting);
            context.SaveChanges();

            var viewModel = CreateCalendarViewModel(context);
            viewModel.Initialize();

            // Act
            await viewModel.DeleteEvent(meeting);

            // Assert
            Assert.Empty(viewModel.Events);
        }

        [Fact]
        public async Task ModifyEvent_ShouldUpdateMeetingAndTriggerNotifications()
        {
            // Arrange
            var context = CreateDbContext();

            var user1 = new User { Id = 1, FullName = "Peter", Color = "#FF0000", Email = "asd@gmail.com", Password = "asd", Token = "asd" };
            var user2 = new User { Id = 2, FullName = "Bob", Color = "#00FF00", Email = "asd@gmail.com", Password = "asd", Token = "asd" };

            var meeting = new Meeting
            {
                Id = 1,
                EventName = "Original Meeting",
                SelectedUsers = new List<User> { user1 },
                From = DateTime.Now,
                To = DateTime.Now.AddHours(1),
                BackgroundColor = "#FF0000"
            };

            var updatedMeeting = new Meeting
            {
                Id = 1,
                EventName = "Updated Meeting",
                SelectedUsers = new List<User> { user2 },
                From = DateTime.Now.AddHours(2),
                To = DateTime.Now.AddHours(3),
                BackgroundColor = "#00FF00"
            };

            context.Users.AddRange(user1, user2);
            context.Meetings.Add(meeting);
            await context.SaveChangesAsync();

            var viewModel = CreateCalendarViewModel(context);
            viewModel.Initialize();

            // Act
            await viewModel.ModifyEvent(updatedMeeting);

            // Assert
            var modifiedMeeting = viewModel.Events.FirstOrDefault(e => e.Id == 1);
            Assert.NotNull(modifiedMeeting);
            Assert.Equal("Updated Meeting", modifiedMeeting.EventName);

            _mockNotificationsService.Verify(n =>
                n.CreateNotification("Updated Meeting", null, NotificationType.MeetingModified, 2), Times.Once);
        }
    }
} 


