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

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadUsers()
        {
            using (var dbContext = new DataContext())
            {
                var users = dbContext.Users.ToList();
                Users = new ObservableCollection<User>(users);
            }
        }
    }
}
