using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskPro.Data.Entidades;

namespace TaskPro.Data
{
    public class DataContext : IdentityDbContext<Usuario>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Tarea> Tareas { get; set; }

        public DbSet<Proyecto> Proyectos { get; set; }
    }
}