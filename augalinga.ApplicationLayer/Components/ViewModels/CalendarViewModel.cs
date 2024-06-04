using System.Collections.ObjectModel;
using System.ComponentModel;
using augalinga.Data.Access;
using augalinga.Data.Entities;

namespace augalinga.ApplicationLayer.Components.ViewModels;

public class CalendarViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Meeting> _events;
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

    }

    public CalendarViewModel(bool baronaite, bool gudaityte, bool both)
    {
        LoadEvents(baronaite, gudaityte, both);
    }

    public void LoadEvents(bool baronaite, bool gudaityte, bool both)
    {
        using (var dbContext = new DataContext())
        {
            IQueryable<Meeting> query = dbContext.Meetings.AsQueryable();

            if (!baronaite)
            {
                query = query.Where(m => m.Employee != "Baronaite");
            }
            if (!gudaityte)
            {
                query = query.Where(m => m.Employee != "Gudaityte");
            }
            if (!both)
            {
                query = query.Where(m => m.Employee != "Both");
            }

            var meetings = query.ToList();
            Events = new ObservableCollection<Meeting>(meetings);
        }
    }

    public void AddEventToCollection(Meeting newMeeting)
    {
        Events.Add(newMeeting);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
