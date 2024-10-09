using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Backend.Services
{
    public class AuthService : IAuthService
    {
        private bool _isLoggedIn;
        public event Action OnChange;

        public bool IsUserLoggedIn()
        {
            return _isLoggedIn;
        }

        public void Login(string username, string password)
        {
            // Replace this with your actual login logic
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // Simulate successful login
                _isLoggedIn = true;
            }
        }

        public void Logout()
        {
            _isLoggedIn = false;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
