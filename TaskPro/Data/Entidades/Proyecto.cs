using System.ComponentModel.DataAnnotations;

namespace TaskPro.Data.Entidades
{
    public class Proyecto
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; } = null!;

        public DateTime Inicio { get; set; }

        public DateTime Final { get; set; }
    }
}