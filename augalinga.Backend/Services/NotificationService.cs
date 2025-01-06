using augalinga.Data.Access;
using augalinga.Data.Entities;
using augalinga.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Backend.Services
{
    public class NotificationService : INotificationService
    {
        private readonly DataContext _dbContext;
        private readonly IAuthService _authService;

        private ObservableCollection<Notification> _notifications;
        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            private set => _notifications = value;
        }

        private static readonly Dictionary<NotificationType, string> NotificationTemplates = new Dictionary<NotificationType, string>
        {
            { NotificationType.ProjectAdded, "Project - Added - {0}" },
            { NotificationType.ProjectModified, "Project - Modified - {0}" },
            { NotificationType.ProjectDeleted, "Project - Deleted - {0}" },
            { NotificationType.DocumentAdded, "Document - Added to {1} - {0}" },
            { NotificationType.DocumentDeleted, "Document - Deleted from {1} - {0}" },
            { NotificationType.DraftAdded, "Draft - Added to {1} - {0}" },
            { NotificationType.DraftDeleted, "Draft - Deleted from {1} - {0}" },
            { NotificationType.OrderAdded, "Order - Added to {1} - {0}" },
            { NotificationType.OrderDeleted, "Order - Deleted from {1} - {0}" },
            { NotificationType.PhotoAdded, "Photo - Added to {1} - {0}" },
            { NotificationType.PhotoDeleted, "Photo - Deleted from {1} - {0}" },
            { NotificationType.ContactAdded, "Contact - Added to {1} - {0}" },
            { NotificationType.ContactDeleted, "Contact - Deleted from {1} - {0}" },
            { NotificationType.MeetingAdded, "Meeting - Added - {0}" },
            { NotificationType.MeetingModified, "Meeting - Modified - {0}" },
            { NotificationType.MeetingDeleted, "Meeting - Deleted - {0}" },
            { NotificationType.UserAdded, "User - Registered - {0}" },
            { NotificationType.UserDeleted, "User - Removed - {0}" },
            { NotificationType.UserModified, "User - Updated - {0}" },
            { NotificationType.ExpenseAdded, "Expense - Added to {1} - {0}" },
            { NotificationType.ExpenseDeleted, "Expense - Deleted from {1} - {0}" }
        };

        public NotificationService(IAuthService authService, DataContext dbContext)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            LoadNotifications();
        }

        public void CreateNotification(string entityName, string? projectName, NotificationType type, int? forUserId)
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null) throw new InvalidOperationException("Current user cannot be null.");

            string messageTemplate = NotificationTemplates.ContainsKey(type) ? NotificationTemplates[type] : "Notification - {0}";
            string message = string.Format(messageTemplate, entityName, projectName);

            var notification = new Notification
            {
                Message = message,
                CreatedAt = DateTime.UtcNow,
                UserId = currentUser.Id,
                ForUserId = forUserId,
                Type = type
            };

            AddNotification(notification);
        }

        private void AddNotification(Notification notification)
        {
            Notifications.Add(notification);
            SaveNotification(notification);
        }

        private void LoadNotifications()
        {
            var notifications = _dbContext.Notifications
                .Include(n => n.User)
                .ToList();

            Notifications = new ObservableCollection<Notification>(notifications);
        }

        private void SaveNotification(Notification notification)
        {
            _dbContext.Notifications.Add(notification);
            _dbContext.SaveChanges();
        }
    }
}
