using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace augalinga.Backend.ViewModels
{
    public class UserLoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;

        [Required(ErrorMessage = "Username is required.")]
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        [Required(ErrorMessage = "Password is required.")]
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
