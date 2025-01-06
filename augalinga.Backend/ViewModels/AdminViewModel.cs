using augalinga.Backend.Services;
using augalinga.Backend.ViewModels;
using augalinga.Data.Access;
using augalinga.Data.Entities;
using augalinga.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace augalinga.Backend.ViewModels
{
    public class AdminViewModel
    {
        private readonly DataContext _dbContext;
        private readonly INotificationService _notificationService;
        private readonly IAuthService _authService;
        private readonly UsersViewModel _usersViewModel;


        public AdminViewModel(UsersViewModel usersViewModel, INotificationService notificationViewModel, IAuthService authService)
        {
            _dbContext = new DataContext();
            _notificationService = notificationViewModel;
            _authService = authService;
            _usersViewModel = usersViewModel;
        }

        public async Task<(bool IsSuccess, string Message)> SaveUserChanges(User selectedUser, string currentPassword, string newPassword)
        {
            var existingUser = await _dbContext.Users.FindAsync(selectedUser.Id);
            if (existingUser == null)
            {
                return (false, "User not found.");
            }

            if (selectedUser.FullName != existingUser.FullName)
            {
                existingUser.FullName = selectedUser.FullName;
                _notificationService.CreateNotification(existingUser.FullName, null, NotificationType.UserModified, null);
            }

            if (selectedUser.Email != existingUser.Email)
            {
                existingUser.Email = selectedUser.Email;
                _notificationService.CreateNotification(existingUser.FullName, null, NotificationType.UserModified, null);
            }

            if (selectedUser.Color != existingUser.Color)
            {
                existingUser.Color = selectedUser.Color;
                _notificationService.CreateNotification(existingUser.FullName, null, NotificationType.UserModified, null);

                var userMeetings = await _dbContext.Meetings
                    .Where(m => m.SelectedUsers.Contains(existingUser))
                    .ToListAsync();

                foreach (var meeting in userMeetings)
                {
                    meeting.BackgroundColor = selectedUser.Color;
                }
            }

            if (!string.IsNullOrWhiteSpace(newPassword) && !string.IsNullOrWhiteSpace(currentPassword))
            {
                if (!BCrypt.Net.BCrypt.Verify(currentPassword, existingUser.Password))
                {
                    return (false, "Current password is incorrect.");
                }
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            await _dbContext.SaveChangesAsync();
            return (true, "User changes saved successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> DeleteUser(User user)
        {
            var existingUser = await _dbContext.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                return (false, "User not found.");
            }

            var notifications = await _dbContext.Notifications
                .Where(n => n.UserId == user.Id || n.ForUserId == user.Id)
                .ToListAsync();

            _dbContext.Notifications.RemoveRange(notifications);

            var meetings = await _dbContext.Meetings
                .Where(m => m.SelectedUsers.Contains(user))
                .ToListAsync();

            foreach (var meeting in meetings)
            {
                meeting.SelectedUsers.Remove(user);

                if (!meeting.SelectedUsers.Any())
                {
                    _dbContext.Meetings.Remove(meeting);
                }
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            _usersViewModel.Users.Remove(user);
            return (true, "User deleted successfully.");
        }
    }
}