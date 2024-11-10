using augalinga.Backend.ViewModels;
using augalinga.Data.Access;
using augalinga.Data.Entities;
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
        private void NotifyStateChanged() => OnChange?.Invoke();
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

        public bool IsUserLoggedIn() => _isLoggedIn;

        public async Task<bool> Login(string email, string password)
        {
            var user = await VerifyUser(email, password);
            if (user != null)
            {
                _isLoggedIn = true;
                _currentUser = user;

                await SecureStorage.SetAsync(AuthTokenKey, user.Token);
                NotifyStateChanged();
                return true;
            }
            return false;
        }

        private async Task<User> VerifyUser(string email, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password) ? user : null;
        }

        public void Logout()
        {
            _isLoggedIn = false;
            _currentUser = null;
            SecureStorage.Remove(AuthTokenKey);
            NotifyStateChanged();
        }

        public User GetCurrentUser() => _currentUser;

        public async Task<bool> RegisterUser(UserRegisterViewModel viewModel)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);
            if (existingUser != null)
            {
                return false; // User already exists
            }

            var newUser = new User
            {
                FullName = viewModel.FullName,
                Email = viewModel.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(viewModel.Password),
                Color = viewModel.Background,
                Token = Guid.NewGuid().ToString()
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();
            await SecureStorage.SetAsync(AuthTokenKey, newUser.Token);

            return true;
        }
    }
}
