using System.Linq;
using AsistenciaApp.Contracts.Services;
using AsistenciaApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaApp.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private string? _authenticatedUser;
        private readonly IDbContextFactory<AssistanceDbContext> _dbContextFactory;

        public AuthenticationService(IDbContextFactory<AssistanceDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public bool IsUserAuthenticated()
        {
            return !string.IsNullOrEmpty(_authenticatedUser);
        }

        public bool AuthenticateUser(string username, string password)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var user = context.Admin.FirstOrDefault(u => u.User == username && u.Password == password);

            if (user == null)
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