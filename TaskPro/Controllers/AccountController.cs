using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using TaskPro.Data;
using TaskPro.Data.Entidades;
using TaskPro.Data.Enums;
using TaskPro.Migrations;
using TaskPro.Models;
using TaskPro.Services;

namespace TaskPro.Controllers
{
    public class AccountController : Controller
    {
        private readonly IServicioUsuario _servicioUsuario;
        private readonly DataContext _context;
        private readonly Cloudinary _cloudinary;

        public AccountController(IServicioUsuario servicioUsuario, DataContext context, Cloudinary cloudinary)
        {
            _servicioUsuario = servicioUsuario;
            _context = context;
            _cloudinary = cloudinary;
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _servicioUsuario.LoginAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _servicioUsuario.LogoutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult Register()
        {
            AddUserViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                TipoUsuario = TipoUsuario.Usuario,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model, IFormFile? file)
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

                // Intentar iniciar sesión automáticamente
                LoginViewModel login = new()
                {
                    Password = model.Password,
                    RememberMe = false,
                    Username = model.Username
                };

                var result2 = await _servicioUsuario.LoginAsync(login);
                if (result2.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al iniciar sesión. Verifique sus credenciales.");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ChangeUser()
        {
            Usuario user = await _servicioUsuario.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel model = new()
            {
                Nombre = user.Nombre,
                Apellidos = user.Apellidos,
                URLFoto = user.URLFoto,
                Id = user.Id,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model, IFormFile? file)
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

                Usuario user = await _servicioUsuario.GetUserAsync(User.Identity!.Name!);

                user.Nombre = model.Nombre;
                user.Apellidos = model.Apellidos;
                user.Id = model.Id;
                user.URLFoto = model.URLFoto ?? user.URLFoto;

                await _servicioUsuario.UpdateUserAsync(user);
                TempData["AlertMessage"] = "Los datos del usuario se actualizaron exitosamente";
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}