using augalinga.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Backend.Services
{
    public interface IAuthService
    {
        event Action OnChange;
        bool IsUserLoggedIn();
        Task<bool> Login(string email, string password);
        void Logout();
        User GetCurrentUser();
        Task InitializeAsync();
        Task<bool> RegisterUser(UserRegisterViewModel viewModel);
        public Task<bool> RequestPasswordResetAsync(string email);
        public Task<bool> ResetPasswordAsync(string resetToken, string newPassword);
    };

}
