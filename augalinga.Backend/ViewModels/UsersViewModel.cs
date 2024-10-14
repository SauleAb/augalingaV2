using augalinga.Data.Access;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class UsersViewModel : INotifyPropertyChanged
    {
        public UsersViewModel()
        {
            LoadUsers();
        }

        private ObservableCollection<User> _users; // Changed from Contact to User
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public void AddUserToCollection(User user)
        {
            Users.Add(user);
            SaveUser(user); // Save the new user to the database
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RemoveUser(User user)
        {
            using (var dbContext = new DataContext())
            {
                dbContext.Users.Remove(user); // Changed from Contacts to Users
                dbContext.SaveChanges();
            }
            LoadUsers(); // Reload all users after removal
        }

        private void LoadUsers() // Method for loading all users
        {
            using (var dbContext = new DataContext())
            {
                var users = dbContext.Users.ToList(); // Changed from Contacts to Users
                Users = new ObservableCollection<User>(users);
            }
        }

        private void SaveUser(User user) // Method to save a new user
        {
            using (var dbContext = new DataContext())
            {
                dbContext.Users.Add(user); // Add the user to the context
                dbContext.SaveChanges(); // Save changes to the database
            }
        }
    }
}
