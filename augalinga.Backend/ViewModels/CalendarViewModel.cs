using augalinga.Data.Access;
using augalinga.Data.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;

public class CalendarViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Meeting> _events;
    private HashSet<User> _selectedUsers; // Use HashSet to manage selected users efficiently
    public List<User> Users { get; set; } // New property to hold users

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
        _selectedUsers = new HashSet<User>(); 
        Users = LoadUsers(); 
        LoadEvents(new List<User>());
    }

    private List<User> LoadUsers()
    {
        using (var dbContext = new DataContext())
        {
            var users = dbContext.Users.ToList();
            return users;
        }
    }

    public void LoadEvents(List<User> selectedUsers)
    {
        using (var dbContext = new DataContext())
        {
            IQueryable<Meeting> query = dbContext.Meetings.AsQueryable();

            if (selectedUsers.Any())
            {
                // Filter events based on selected users
                query = query.Where(m => selectedUsers.Contains(m.User));
            }
            var meetings = query.ToList();
            Events = new ObservableCollection<Meeting>(meetings);
        }
    }

    public void ToggleUserSelection(User user)
    {
        if (_selectedUsers.Contains(user))
        {
            _selectedUsers.Remove(user);
        }
        else
        {
            _selectedUsers.Add(user);
        }

        // Load events based on updated selected users
        LoadEvents(_selectedUsers.ToList());
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
