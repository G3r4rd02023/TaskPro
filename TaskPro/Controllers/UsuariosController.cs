using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskPro.Data;
using TaskPro.Data.Entidades;
using TaskPro.Data.Enums;
using TaskPro.Models;
using TaskPro.Services;

namespace TaskPro.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly IServicioUsuario _servicioUsuario;
        private readonly DataContext _context;
        private readonly Cloudinary _cloudinary;

        public UsuariosController(IServicioUsuario servicioUsuario, DataContext context, Cloudinary cloudinary)
        {
            _servicioUsuario = servicioUsuario;
            _context = context;
            _cloudinary = cloudinary;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
            .ToListAsync());
        }

        public IActionResult Create()
        {
            AddUserViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                TipoUsuario = TipoUsuario.Administrador,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // Manejar la carga de la imagen solo si hay un archivo adjunto
                if (file != null)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream()),
                        AssetFolder = "tecnologers"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    // Verificar si la carga fue exitosa
                    if (uploadResult.Error != null)
                    {
                        ModelState.AddModelError(string.Empty, "Error al cargar la imagen.");
                        return View(model);
                    }

                    var urlImagen = uploadResult.SecureUrl.ToString();
                    model.URLFoto = urlImagen;
                }

                // Verificar si el correo ya está en uso antes de crear el usuario
                Usuario user = await _servicioUsuario.AddUserAsync(model);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
                    return View(model);
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}