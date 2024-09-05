using TaskPro.Data.Entidades;
using TaskPro.Data.Enums;
using TaskPro.Services;

namespace TaskPro.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IServicioUsuario _servicioUsuario;

        public SeedDb(DataContext context, IServicioUsuario servicioUsuario)
        {
            _context = context;
            _servicioUsuario = servicioUsuario;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckUserAsync("Super", "Admin", "tecnologerhn@gmail.com",
            TipoUsuario.Administrador);
        }

        private async Task<Usuario> CheckUserAsync(string nombre, string apellidos, string email, TipoUsuario tipoUsuario)
        {
            Usuario user = await _servicioUsuario.GetUserAsync(email);
            if (user == null)
            {
                user = new Usuario
                {
                    Nombre = nombre,
                    Apellidos = apellidos,
                    Email = email,
                    UserName = email,
                    TipoUsuario = tipoUsuario,
                };
                await _servicioUsuario.AddUserAsync(user, "123456");
                await _servicioUsuario.AddUserToRoleAsync(user, tipoUsuario.ToString());
            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _servicioUsuario.CheckRoleAsync(TipoUsuario.Administrador.ToString());
            await _servicioUsuario.CheckRoleAsync(TipoUsuario.Usuario.ToString());
        }
    }
}