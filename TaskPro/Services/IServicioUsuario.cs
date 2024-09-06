using Microsoft.AspNetCore.Identity;
using TaskPro.Data.Entidades;
using TaskPro.Models;

namespace TaskPro.Services
{
    public interface IServicioUsuario
    {
        Task<Usuario> GetUserAsync(string email);

        Task<IdentityResult> AddUserAsync(Usuario user, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(Usuario user, string roleName);

        Task<bool> IsUserInRoleAsync(Usuario user, string roleName);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<Usuario> AddUserAsync(AddUserViewModel model);

        Task<IdentityResult> ChangePasswordAsync(Usuario user, string oldPassword, string newPassword);

        Task<IdentityResult> UpdateUserAsync(Usuario user);

        Task<Usuario> GetUserAsync(Guid userId);
    }
}