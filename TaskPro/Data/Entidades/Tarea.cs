using System.ComponentModel.DataAnnotations;
using TaskPro.Data.Enums;

namespace TaskPro.Data.Entidades
{
    public class Tarea
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; } = null!;

        public DateTime FechaRegistro { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public Estado Estado { get; set; }

        public Prioridad Prioridad { get; set; }

        public Proyecto? Proyecto { get; set; }

        public int ProyectoId { get; set; }
    }
}