using augalinga.Data.Entities;
using augalinga.Data.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Backend.Services
{
    public interface INotificationService
    {
        ObservableCollection<Notification> Notifications { get; }
        void CreateNotification(string entityName, string? projectName, NotificationType type, int? forUserId);
    }
}
