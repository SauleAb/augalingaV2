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
            { NotificationType.ProjectAdded, "A new project has been added - {0}" },
            { NotificationType.ProjectModified, "A project has been modified - {0}" },
            { NotificationType.ProjectDeleted, "A project has been deleted - {0}" },
            { NotificationType.DocumentAdded, "A new document has been added to {1} - {0}" },
            { NotificationType.DocumentDeleted, "A document has been deleted from {1} - {0}" },
            { NotificationType.DraftAdded, "A new draft has been added to {1} - {0}" },
            { NotificationType.DraftDeleted, "A draft has been deleted from {1} - {0}" },
            { NotificationType.OrderAdded, "A new order has been added to {1} - {0}" },
            { NotificationType.OrderDeleted, "An order has been deleted from {1} - {0}" },
            { NotificationType.PhotoAdded, "A new photo has been added to {1} - {0}" },
            { NotificationType.PhotoDeleted, "A photo has been deleted from {1} - {0}" },
            { NotificationType.ContactAdded, "A new contact has been added to {1} - {0}" },
            { NotificationType.ContactDeleted, "A contact has been deleted from {1} - {0}" },
            { NotificationType.MeetingAdded, "A new meeting has been added - {0}" },
            { NotificationType.MeetingModified, "A meeting has been modified - {0}" },
            { NotificationType.MeetingDeleted, "A meeting has been deleted - {0}" },
            { NotificationType.UserAdded, "A new user has been registered - {0}" },
            { NotificationType.UserDeleted, "User has been removed - {0}" },
            { NotificationType.UserModified, "User's details have been updated - {0}" },
            { NotificationType.ExpenseAdded, "A new expense has been added to {1} - {0}" },
            { NotificationType.ExpenseDeleted, "An expense has been deleted from {1} - {0}" }
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
