//using augalinga.Backend.Services;
//using augalinga.Data.Access;
//using augalinga.Data.Entities;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//public class NotificationService
//{
//    private readonly DataContext _dbContext;
//    private readonly IAuthService _authService;

//    public NotificationService(DataContext dbContext, IAuthService authService)
//    {
//        _dbContext = dbContext;
//        _authService = authService;
//    }

//    public async Task<List<Notification>> GetAllNotificationsAsync()
//    {
//        return await _dbContext.Notifications
//            .Include(n => n.User)
//            .ToListAsync();
//    }

//    public async Task CreateNotificationAsync(string pageName)
//    {
//        var currentUser = _authService.GetCurrentUser();

//        var notification = new Notification
//        {
//            PageName = pageName,
//            CreatedAt = DateTime.UtcNow,
//            UserId = currentUser.Id
//        };

//        _dbContext.Notifications.Add(notification);
//        await _dbContext.SaveChangesAsync();
//    }

//    public async Task DeleteNotificationAsync(int notificationId)
//    {
//        var notification = await _dbContext.Notifications.FindAsync(notificationId);
//        if (notification != null)
//        {
//            _dbContext.Notifications.Remove(notification);
//            await _dbContext.SaveChangesAsync();
//        }
//    }
//}
