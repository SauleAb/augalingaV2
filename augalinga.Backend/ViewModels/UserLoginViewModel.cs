using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace augalinga.Backend.ViewModels
{
    public class UserLoginViewModel : INotifyPropertyChanged
    {
        private string _email;
        private string _password;

        [Required(ErrorMessage = "Email is required.")]
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
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
