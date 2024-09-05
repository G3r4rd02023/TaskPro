using Microsoft.EntityFrameworkCore;
using TaskPro.Data.Entidades;

namespace TaskPro.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Tarea> Tareas { get; set; }

        public DbSet<Proyecto> Proyectos { get; set; }
    }
}