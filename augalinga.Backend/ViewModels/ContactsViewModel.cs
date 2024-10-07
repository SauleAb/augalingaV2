using augalinga.Data.Access;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Contact = augalinga.Data.Entities.Contact;

namespace augalinga.Backend.ViewModels
{
    public class ContactsViewModel : INotifyPropertyChanged
    {
        string _category;
        public ContactsViewModel(string category)
        {
            _category = category;
            LoadContacts(_category);
        }

        public ContactsViewModel()
        {
            LoadContacts();
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

        public void AddContactToCollection(Contact contact)
        {
            Contacts.Add(contact);
            LoadContacts(_category);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RemoveContact(Contact contact)
        {
            using (var dbContext = new DataContext())
            {
                dbContext.Contacts.Remove(contact);
                dbContext.SaveChanges();
            }
            LoadContacts(_category);
        }

        private void LoadContacts(string category)
        {
            using (var dbContext = new DataContext())
            {
                var contacts = dbContext.Contacts
                    .Where(c => c.Category == category)
                    .ToList();

                Contacts = new ObservableCollection<Contact>(contacts);
            }
        }

        private void LoadContacts()
        {
            using (var dbContext = new DataContext())
            {
                var contacts = dbContext.Contacts.ToList();

                Contacts = new ObservableCollection<Contact>(contacts);
            }
        }
    }
}