using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsistenciaApp.Contracts.Services
{
    public interface IAuthenticationService
    {
        bool IsUserAuthenticated();
        bool AuthenticateUser(string username, string password);
        void Logout();
        string GetAuthenticatedUser();
    }
}
