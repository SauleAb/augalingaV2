﻿using augalinga.Data.Access;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Contact = augalinga.Data.Entities.Contact;

namespace augalinga.Backend.ViewModels
{
    public class ProjectContactsViewModel : INotifyPropertyChanged
    {
        int _projectId;
        public ProjectContactsViewModel(int projectId)
        {
            _projectId = projectId;
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
            using (var dbContext = new DataContext())
            {
                var contacts = dbContext.Contacts
                    .Where(c => c.ProjectId == projectId)
                    .ToList();

                Contacts = new ObservableCollection<Contact>(contacts);
            }
        }
    }
}