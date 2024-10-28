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
            SaveUser(user);
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
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();
            }
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var dbContext = new DataContext())
            {
                var users = dbContext.Users.ToList();
                Users = new ObservableCollection<User>(users);
            }
        }

        private void SaveUser(User user)
        {
            using (var dbContext = new DataContext())
            {
                dbContext.Users.Add(user); 
                dbContext.SaveChanges(); 
            }
        }
    }
}
