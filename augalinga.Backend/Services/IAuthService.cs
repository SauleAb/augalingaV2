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
        Task Login(string fullName, string password);
        void Logout();
        User GetCurrentUser();
    }

}
