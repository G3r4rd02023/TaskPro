using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskPro.Data;
using TaskPro.Data.Entidades;
using TaskPro.Models;

namespace TaskPro.Services
{
    public class ServicioUsuario : IServicioUsuario
    {
        private readonly DataContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Usuario> _signInManager;

        public ServicioUsuario(DataContext context, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Usuario> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddUserAsync(Usuario user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<Usuario> AddUserAsync(AddUserViewModel model)
        {
            Usuario user = new()
            {
                Email = model.Username,
                UserName = model.Username,
                Nombre = model.Nombre,
                Apellidos = model.Apellidos,
                URLFoto = model.URLFoto,
                TipoUsuario = model.TipoUsuario
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result != IdentityResult.Success)
            {
                return null!;
            }

            Usuario newUser = await GetUserAsync(model.Username);
            await AddUserToRoleAsync(newUser, user.TipoUsuario.ToString());
            return newUser;
        }

        public async Task AddUserToRoleAsync(Usuario user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(Usuario user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<Usuario> GetUserAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario> GetUserAsync(Guid userId)
        {
            return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId.ToString());
        }

        public async Task<bool> IsUserInRoleAsync(Usuario user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
            model.Username,
            model.Password,
            model.RememberMe,
            false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> UpdateUserAsync(Usuario user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}