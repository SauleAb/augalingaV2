using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace augalinga.Backend.ViewModels
{
    public class UserLoginViewModel : INotifyPropertyChanged
    {
        private string _fullName;
        private string _password;

        [Required(ErrorMessage = "Full name is required.")]
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
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
