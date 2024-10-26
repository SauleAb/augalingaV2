using augalinga.Backend.Services;
using augalinga.Data.Access;
using augalinga.Data.Entities; // Assuming Notification entity is in this namespace
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

        public void CreateNotification(string pageName)
        {
            var currentUser = _authService.GetCurrentUser();

            var notification = new Notification
            {
                PageName = pageName,
                CreatedAt = DateTime.UtcNow,
                UserId = currentUser.Id
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
