using augalinga.Data.Access;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Contact = augalinga.Data.Entities.Contact;

namespace augalinga.Backend.ViewModels
{
    public class ContactsViewModel : INotifyPropertyChanged
    {
        string _category;
        private readonly DataContext _dbContext;
        public ContactsViewModel(string category, DataContext dbContext)
        {
            _category = category; 
            _dbContext = dbContext;
            LoadContacts(_category);
        }

        public ContactsViewModel(DataContext dbContext)
        {
            _dbContext = dbContext;
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

        public async void AddContact(Contact contact)
        {
            _dbContext.Contacts.Add(contact);
            await _dbContext.SaveChangesAsync();
            LoadContacts(_category);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void RemoveContact(Contact contact)
        {
            _dbContext.Contacts.Remove(contact);
            await _dbContext.SaveChangesAsync();
            LoadContacts(_category);
        }

        public void SetDefaultValues(Contact contact)
        {
            if (contact.Notes == null)
            {
                contact.Notes = "";
            }
            if (contact.Address == null)
            {
                contact.Address = "";
            }
        }

        public void LoadContacts(string category)
        {
            var contacts = _dbContext.Contacts
                .Where(c => c.Category == category)
                .ToList();

            Contacts = new ObservableCollection<Contact>(contacts);
        }

        public void LoadContacts()
        {
            var contacts = _dbContext.Contacts.ToList();

            Contacts = new ObservableCollection<Contact>(contacts);
        }
    }
}