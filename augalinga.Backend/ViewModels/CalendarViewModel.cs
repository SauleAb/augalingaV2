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
    private readonly NotificationsViewModel _notificationsViewModel;
    public event PropertyChangedEventHandler PropertyChanged;
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

    public CalendarViewModel(NotificationsViewModel notificationsViewModel)
    {
        _notificationsViewModel = notificationsViewModel;
        _selectedUserIds = new HashSet<int>();
        Users = LoadUsers();
        Events = new ObservableCollection<Meeting>();
        LoadEvents(_selectedUserIds.ToList());
    }

    private List<User> LoadUsers()
    {
        using (var dbContext = new DataContext())
        {
            return dbContext.Users.Include(u => u.Meetings).ToList();
        }
    }

    public void LoadEvents(List<int> selectedUserIds)
    {
        using (var dbContext = new DataContext())
        {
            Events.Clear();

            if (selectedUserIds.Any())
            {
                var meetings = dbContext.Meetings
                    .Include(m => m.SelectedUsers)
                    .Where(m => m.SelectedUsers.Any(u => selectedUserIds.Contains(u.Id)))
                    .ToList();

                Events = new ObservableCollection<Meeting>(meetings);
            }
            else
            {
                var meetings = dbContext.Meetings.Include(m => m.SelectedUsers).ToList();
                Events = new ObservableCollection<Meeting>(meetings);
            }
        }
    }

    public void CreateNotifications(string meetingTitle, List<User> selectedUsers, NotificationType notificationType)
    {
        foreach (var user in selectedUsers)
        {
            _notificationsViewModel.CreateNotification(meetingTitle, null, notificationType, user.Id);
        }
    }


    public async Task CreateEvent(Meeting addedEvent)
    {
        using (var dbContext = new DataContext())
        {
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
                ? await dbContext.Users.ToListAsync()
                : await dbContext.Users
                    .Where(u => addedEvent.SelectedUsers.Select(su => su.Id).Contains(u.Id))
                    .ToListAsync();

            dbContext.Meetings.Add(meeting);
            await dbContext.SaveChangesAsync();

            Events.Add(meeting);

            CreateNotifications(meeting.EventName, meeting.SelectedUsers, NotificationType.MeetingAdded);
        }
    }

    public async Task DeleteEvent(Meeting deletedEvent)
    {
        using (var dbContext = new DataContext())
        {
            var eventToRemove = await dbContext.Meetings.Include(m => m.SelectedUsers).FirstOrDefaultAsync(e => e.Id == deletedEvent.Id);
            if (eventToRemove == null) return;

            foreach (var user in eventToRemove.SelectedUsers)
            {
                user.Meetings.Remove(eventToRemove);
            }

            dbContext.Meetings.Remove(eventToRemove);
            await dbContext.SaveChangesAsync();
            Events.Remove(eventToRemove);
            CreateNotifications(eventToRemove.EventName, eventToRemove.SelectedUsers, NotificationType.MeetingDeleted);
        }
    }

    public async Task ModifyEvent(Meeting editedEvent)
    {
        using (var dbContext = new DataContext())
        {
            var existingEvent = await dbContext.Meetings.Include(m => m.SelectedUsers)
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
                ? await dbContext.Users.ToListAsync()
                : await dbContext.Users
                    .Where(u => editedEvent.SelectedUsers.Select(su => su.Id).Contains(u.Id))
                    .ToListAsync();

            existingEvent.BackgroundColor = editedEvent.SelectedUsers.Count == 1
                ? editedEvent.SelectedUsers.First().Color
                : "#000000";

            dbContext.Meetings.Update(existingEvent);
            await dbContext.SaveChangesAsync();

            CreateNotifications(existingEvent.EventName, existingEvent.SelectedUsers, NotificationType.MeetingModified);
        }
    }

    public void ToggleAssignToAllUsers(Meeting meeting)
    {
        if (meeting != null)
        {
            if (meeting.IsAssignedToAllUsers)
            {
                meeting.SelectedUsers = Users.ToList(); // Assign all users
            }
            else
            {
                meeting.SelectedUsers.Clear(); // Clear selected users
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
