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
        void Login(string username, string password);
        void Logout();
    }

}
