using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsistenciaApp.Contracts.Services;
using AsistenciaApp.Core.Models;
using Windows.System;

namespace AsistenciaApp.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private string? _authenticatedUser;
        private readonly AssistanceDbContext _dbContext;

        public AuthenticationService(AssistanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsUserAuthenticated()
        {
            return !string.IsNullOrEmpty(_authenticatedUser);
        }

        public bool AuthenticateUser(string username, string password)
        {
            var user = _dbContext.Admin.FirstOrDefault(u => u.User == username && u.Password == password);

            if (user == default)
            {
                throw new UnauthorizedAccessException("Usuario o contraseña inválidos.");
            }

            _authenticatedUser = username;
            return true;    

        }

        public void Logout()
        {
            _authenticatedUser = null;
        }

        public string GetAuthenticatedUser()
        {
            return _authenticatedUser ?? "N/A";
        }
    }
}
