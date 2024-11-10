using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace augalinga.Backend.ViewModels
{
    public class UserRegisterViewModel : INotifyPropertyChanged
    {
        private string _fullName;
        private string _password;
        private string _email;
        private string _background = "#FFFFFF";

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
        [DataType(DataType.Password)]
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        [Required(ErrorMessage = "Color selection is required.")]
        public string Background
        {
            get => _background;
            set
            {
                _background = value;
                OnPropertyChanged(nameof(Background));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
