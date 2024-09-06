using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TaskPro.Data.Enums;

namespace TaskPro.Data.Entidades
{
    public class Usuario : IdentityUser
    {
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Apellidos { get; set; } = null!;

        public string? URLFoto { get; set; }

        public TipoUsuario TipoUsuario { get; set; }

        [Display(Name = "Usuario")]
        public string NombreCompleto => $"{Nombre} {Apellidos}";
    }
}