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
    public ObservableCollection<Meeting> Events
    {
        get => _events;
        set
        {
            _events = value;
            OnPropertyChanged(nameof(Events));
        }
    }

    public CalendarViewModel()
    {
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

    public async Task CreateEvent(Meeting addedEvent)
    {
        using (var dbContext = new DataContext())
        {
            var meeting = new Meeting
            {
                From = addedEvent.From,
                To = addedEvent.To,
                EventName = addedEvent.EventName,
                IsAllDay = addedEvent.IsAllDay,
                Notes = addedEvent.Notes,
                BackgroundColor = addedEvent.SelectedUsers.Count == 1
                    ? addedEvent.SelectedUsers.First().Color
                    : "#000000"
            };

            // Assign SelectedUsers with context-managed entities
            meeting.SelectedUsers = addedEvent.IsAssignedToAllUsers
                ? await dbContext.Users.ToListAsync()
                : await dbContext.Users
                    .Where(u => addedEvent.SelectedUsers.Select(su => su.Id).Contains(u.Id))
                    .ToListAsync();

            dbContext.Meetings.Add(meeting);
            await dbContext.SaveChangesAsync();

            Events.Add(meeting);
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
                //NotificationsViewModel.CreateNotification(eventToRemove.EventName, null, NotificationType.MeetingDeleted, user.Id);
            }

            dbContext.Meetings.Remove(eventToRemove);
            await dbContext.SaveChangesAsync();
            Events.Remove(eventToRemove);
        }
    }

    public async Task ModifyEvent(Meeting editedEvent)
    {
        using (var dbContext = new DataContext())
        {
            var existingEvent = await dbContext.Meetings.Include(m => m.SelectedUsers)
                .FirstOrDefaultAsync(e => e.Id == editedEvent.Id);

            if (existingEvent == null) return;

            existingEvent.From = editedEvent.From;
            existingEvent.To = editedEvent.To;
            existingEvent.IsAllDay = editedEvent.IsAllDay;
            existingEvent.EventName = editedEvent.EventName;
            existingEvent.Notes = editedEvent.Notes;
            existingEvent.IsAssignedToAllUsers = editedEvent.IsAssignedToAllUsers;

            // Reassign SelectedUsers with context-managed entities
            existingEvent.SelectedUsers = editedEvent.IsAssignedToAllUsers
                ? await dbContext.Users.ToListAsync()
                : await dbContext.Users
                    .Where(u => editedEvent.SelectedUsers.Select(su => su.Id).Contains(u.Id))
                    .ToListAsync();

            existingEvent.BackgroundColor = editedEvent.SelectedUsers.Count == 1
                ? editedEvent.SelectedUsers.First().Color
                : "#000000";

            dbContext.Meetings.Update(existingEvent);
            await Task.Delay(500); // Optional delay
            await dbContext.SaveChangesAsync();
        }
    }

    public void ToggleUserSelection(int userId)
    {
        if (_selectedUserIds.Contains(userId))
        {
            _selectedUserIds.Remove(userId);
        }
        else
        {
            _selectedUserIds.Add(userId);
        }

        LoadEvents(_selectedUserIds.ToList());
    }

    public async Task AssignMeetingToUsers(Meeting meeting)
    {
        using (var dbContext = new DataContext())
        {
            if (meeting.IsAssignedToAllUsers)
            {
                meeting.SelectedUsers = await dbContext.Users.ToListAsync();
            }
            else
            {
                meeting.SelectedUsers = await dbContext.Users
                    .Where(u => meeting.SelectedUsers.Select(su => su.Id).Contains(u.Id))
                    .ToListAsync();
            }

            dbContext.Meetings.Add(meeting);
            await dbContext.SaveChangesAsync();
            Events.Add(meeting);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
