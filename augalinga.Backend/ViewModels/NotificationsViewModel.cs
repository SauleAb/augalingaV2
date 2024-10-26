using augalinga.Data.Access;
using augalinga.Data.Entities; // Assuming Notification entity is in this namespace
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class NotificationsViewModel : INotifyPropertyChanged
    {
        public NotificationsViewModel()
        {
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

        public void AddNotificationToCollection(Notification notification)
        {
            Notifications.Add(notification);
            SaveNotification(notification); // Save the new notification to the database
        }

        public void RemoveNotification(Notification notification)
        {
            using (var dbContext = new DataContext())
            {
                dbContext.Notifications.Remove(notification);
                dbContext.SaveChanges();
            }
            LoadNotifications(); // Reload all notifications after removal
        }

        private void LoadNotifications() // Method for loading all notifications
        {
            using (var dbContext = new DataContext())
            {
                var notifications = dbContext.Notifications.ToList();
                Notifications = new ObservableCollection<Notification>(notifications);
            }
        }

        private void SaveNotification(Notification notification) // Method to save a new notification
        {
            using (var dbContext = new DataContext())
            {
                dbContext.Notifications.Add(notification);
                dbContext.SaveChanges(); // Save changes to the database
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}