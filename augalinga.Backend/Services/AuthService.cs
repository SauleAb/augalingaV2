using augalinga.Data.Access;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _dbContext = new DataContext();
        private bool _isLoggedIn;
        private User _currentUser;
        public event Action OnChange;

        public bool IsUserLoggedIn()
        {
            return _isLoggedIn;
        }

        public async Task Login(string fullName, string password)
        {
            if (!string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(password))
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.FullName == fullName);

                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    _isLoggedIn = true;
                    _currentUser = user;
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
            NotifyStateChanged();
        }

        public User GetCurrentUser()
        {
            return _currentUser;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
