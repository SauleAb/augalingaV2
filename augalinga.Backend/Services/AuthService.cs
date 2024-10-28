using augalinga.Data.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Storage; // Add this namespace for SecureStorage
using System;
using System.Threading.Tasks;

namespace augalinga.Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _dbContext = new DataContext();
        private bool _isLoggedIn;
        private User _currentUser;
        public event Action OnChange;

        private const string AuthTokenKey = "authToken";

        public async Task InitializeAsync()
        {
            var storedToken = await SecureStorage.GetAsync(AuthTokenKey);
            if (!string.IsNullOrEmpty(storedToken))
            {
                _currentUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Token == storedToken);
                _isLoggedIn = _currentUser != null;
                NotifyStateChanged();
            }
        }

        public bool IsUserLoggedIn()
        {
            return _isLoggedIn;
        }

        public async Task Login(string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    _isLoggedIn = true;
                    _currentUser = user;

                    await SecureStorage.SetAsync(AuthTokenKey, user.Token);

                    NotifyStateChanged();
                }
                else
                {
                    _isLoggedIn = false;
                    _currentUser = null;
                }
            }
        }

        public void Logout()
        {
            _isLoggedIn = false;
            _currentUser = null;

            SecureStorage.Remove(AuthTokenKey);
            NotifyStateChanged();
        }
        public User GetCurrentUser()
        {
            return _currentUser;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
