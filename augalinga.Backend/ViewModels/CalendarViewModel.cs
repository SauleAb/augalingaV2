using augalinga.Backend.Services;
using augalinga.Backend.ViewModels;
using augalinga.Data.Access;
using augalinga.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;

public class CalendarViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Meeting> _events;
    public List<User> Users { get; set; }
    private HashSet<int> _selectedUserIds;
    private readonly INotificationService _notificationsService;
    public event PropertyChangedEventHandler PropertyChanged;
    private DataContext _dbContext;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public ObservableCollection<Meeting> Events
    {
        get => _events;
        set
        {
            _events = value;
            OnPropertyChanged(nameof(Events));
        }
    }

    public void Initialize()
    {
        Users = LoadUsers();
        LoadEvents(_selectedUserIds.ToList());
    }

    public CalendarViewModel(INotificationService notificationsService, DataContext dbContext)
    {
        _notificationsService = notificationsService;
        _dbContext = dbContext;
        _selectedUserIds = new HashSet<int>();
        Events = new ObservableCollection<Meeting>();
    }

    private List<User> LoadUsers()
    {
        if (_dbContext == null)
            throw new InvalidOperationException("DataContext is not initialized.");

        return _dbContext.Users.Include(u => u.Meetings).ToList();
    }

    public void LoadEvents(List<int> selectedUserIds)
    {
        if (_dbContext == null)
            throw new InvalidOperationException("DataContext is not initialized.");

        Events.Clear();

        if (selectedUserIds.Any())
        {
            var meetings = _dbContext.Meetings
                .Include(m => m.SelectedUsers)
                .Where(m => m.SelectedUsers.Any(u => selectedUserIds.Contains(u.Id)))
                .ToList();

            Events = new ObservableCollection<Meeting>(meetings);
        }
        else
        {
            var meetings = _dbContext.Meetings.ToList();
            Events = new ObservableCollection<Meeting>(meetings);
        }
    }
    public void CreateNotifications(string meetingTitle, List<User>? selectedUsers, NotificationType notificationType)
    {
        if (_notificationsService == null)
            throw new InvalidOperationException("NotificationService is not initialized.");
        if (selectedUsers != null)
        {
            foreach (var user in selectedUsers)
            {
                _notificationsService.CreateNotification(meetingTitle, null, notificationType, user.Id);
            }
        }
        else
        {
            _notificationsService.CreateNotification(meetingTitle, null, notificationType, null);
        }
    }


    public async Task CreateEvent(Meeting addedEvent)
    {
        if (_dbContext == null)
            throw new InvalidOperationException("DataContext is not initialized.");

        if ((addedEvent.SelectedUsers == null || (!addedEvent.SelectedUsers.Any()) && !addedEvent.IsAssignedToAllUsers) ||
            addedEvent.SelectedUsers.Count == Users.Count)
        {
            addedEvent.IsAssignedToAllUsers = true;
        }

        var meeting = new Meeting
        {
            From = addedEvent.From,
            To = addedEvent.To,
            EventName = addedEvent.EventName,
            IsAllDay = addedEvent.IsAllDay,
            Notes = addedEvent.Notes,
            IsAssignedToAllUsers = addedEvent.IsAssignedToAllUsers,
            BackgroundColor = addedEvent.SelectedUsers.Count == 1
                ? addedEvent.SelectedUsers.First().Color
                : "#000000"
        };

        meeting.SelectedUsers = addedEvent.IsAssignedToAllUsers
            ? await _dbContext.Users.ToListAsync()
            : await _dbContext.Users
                .Where(u => addedEvent.SelectedUsers.Select(su => su.Id).Contains(u.Id))
                .ToListAsync();

        _dbContext.Meetings.Add(meeting);
        await _dbContext.SaveChangesAsync();

        Events.Add(meeting);

        CreateNotifications(meeting.EventName, meeting.SelectedUsers, NotificationType.MeetingAdded);
        LoadEvents(_selectedUserIds.ToList());
    }


    public async Task DeleteEvent(Meeting deletedEvent)
    {
        if (_dbContext == null)
            throw new InvalidOperationException("DataContext is not initialized.");

        var eventToRemove = await _dbContext.Meetings.Include(m => m.SelectedUsers)
            .FirstOrDefaultAsync(e => e.Id == deletedEvent.Id);
        if (eventToRemove == null) return;

        foreach (var user in eventToRemove.SelectedUsers)
        {
            user.Meetings.Remove(eventToRemove);
        }

        _dbContext.Meetings.Remove(eventToRemove);
        await _dbContext.SaveChangesAsync();

        Events.Remove(eventToRemove);

        if ((eventToRemove.SelectedUsers != null && eventToRemove.SelectedUsers.Any()))
        {
            CreateNotifications(eventToRemove.EventName, eventToRemove.SelectedUsers, NotificationType.MeetingDeleted);
        }
        else
        {
            CreateNotifications(eventToRemove.EventName, null, NotificationType.MeetingDeleted);
        }

        LoadEvents(_selectedUserIds.ToList());
    }

    public async Task ModifyEvent(Meeting editedEvent)
    {
        if (_dbContext == null)
            throw new InvalidOperationException("DataContext is not initialized.");

        var existingEvent = await _dbContext.Meetings.Include(m => m.SelectedUsers)
            .FirstOrDefaultAsync(e => e.Id == editedEvent.Id);

        if (existingEvent == null) return;

        if ((editedEvent.SelectedUsers == null || (!editedEvent.SelectedUsers.Any()) && !editedEvent.IsAssignedToAllUsers) ||
            (editedEvent.SelectedUsers.Count == Users.Count))
        {
            editedEvent.IsAssignedToAllUsers = true;
        }

        existingEvent.From = editedEvent.From;
        existingEvent.To = editedEvent.To;
        existingEvent.IsAllDay = editedEvent.IsAllDay;
        existingEvent.EventName = editedEvent.EventName;
        existingEvent.Notes = editedEvent.Notes;
        existingEvent.IsAssignedToAllUsers = editedEvent.IsAssignedToAllUsers;

        existingEvent.SelectedUsers = editedEvent.IsAssignedToAllUsers
            ? await _dbContext.Users.ToListAsync()
            : await _dbContext.Users
                .Where(u => editedEvent.SelectedUsers.Select(su => su.Id).Contains(u.Id))
                .ToListAsync();

        existingEvent.BackgroundColor = editedEvent.SelectedUsers.Count == 1
            ? editedEvent.SelectedUsers.First().Color
            : "#000000";

        _dbContext.Meetings.Update(existingEvent);
        await _dbContext.SaveChangesAsync();

        CreateNotifications(existingEvent.EventName, existingEvent.SelectedUsers, NotificationType.MeetingModified);
        LoadEvents(_selectedUserIds.ToList());
    }

    public void ToggleAssignToAllUsers(Meeting meeting)
    {
        if (meeting != null)
        {
            if (meeting.IsAssignedToAllUsers)
            {
                meeting.SelectedUsers = Users.ToList();
            }
            else
            {
                meeting.SelectedUsers.Clear();
            }
        }
    }

    public void IsAssignToAllUsersCheck(Meeting meeting)
    {
        if (meeting.SelectedUsers.Count == Users.Count)
        {
            meeting.IsAssignedToAllUsers = true;
        }
        else
        {
            meeting.IsAssignedToAllUsers = false;
        }
    }
}
