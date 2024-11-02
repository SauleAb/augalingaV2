using augalinga.Backend.Services;
using augalinga.Data.Access;
using augalinga.Data.Entities; // Assuming Notification entity is in this namespace
using augalinga.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace augalinga.Backend.ViewModels
{
    public class NotificationsViewModel : INotifyPropertyChanged
    {
        private readonly DataContext _dbContext;
        private readonly IAuthService _authService;
        public NotificationsViewModel(IAuthService authService)
        {
            _dbContext = new DataContext();
            _authService = authService;
            LoadNotifications();
        }

        private ObservableCollection<Notification> _notifications;
        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                OnPropertyChanged(nameof(Notifications));
            }
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

        public void CreateNotification(string entityName, string? projectName, NotificationType type, int? forUserId)
        {
            var currentUser = _authService.GetCurrentUser();

            // Format the message using the template for the given notification type
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

            AddNotificationToCollection(notification);
        }

        public void AddNotificationToCollection(Notification notification)
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
