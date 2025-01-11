using augalinga.Data.Access;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Contact = augalinga.Data.Entities.Contact;

namespace augalinga.Backend.ViewModels
{
    public class ProjectContactsViewModel : INotifyPropertyChanged
    {
        int _projectId;
        private readonly DataContext _dbContext;

        public ProjectContactsViewModel(int projectId, DataContext dbContext)
        {
            _projectId = projectId;
            _dbContext = dbContext;
            LoadContacts(_projectId);
        }

        private ObservableCollection<Contact> _contacts;
        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set
            {
                _contacts = value;
                OnPropertyChanged(nameof(Contacts));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadContacts(int projectId)
        {
            var contacts = _dbContext.Contacts
                .Where(c => c.ProjectId == projectId)
                .ToList();

            Contacts = new ObservableCollection<Contact>(contacts);
        }

        public async void AddContact(Contact contact)
        {
            _dbContext.Contacts.Add(contact);
            await _dbContext.SaveChangesAsync();
            LoadContacts(_projectId);
        }

        public async void RemoveContact(Contact contact)
        {
            _dbContext.Contacts.Remove(contact);
            await _dbContext.SaveChangesAsync();
            LoadContacts(_projectId);
        }
    }
}