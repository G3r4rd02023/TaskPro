using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TaskPro.Data;
using TaskPro.Data.Entidades;
using TaskPro.Data.Enums;
using TaskPro.Services;

namespace TaskPro.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ProyectosController : Controller
    {
        private readonly DataContext _context;
        private readonly IServicioUsuario _usuario;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ProyectosController(DataContext context, IServicioUsuario usuario, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _usuario = usuario;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Proyectos.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            var user = await _usuario.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            Proyecto proyecto = new()
            {
                Usuario = user,
                Inicio = DateTime.Now,
                Final = DateTime.Now.AddDays(15)
            };
            return View(proyecto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Proyecto proyecto)
        {
            if (ModelState.IsValid)
            {
                var user = await _usuario.GetUserAsync(User.Identity!.Name!);
                if (user == null)
                {
                    return NotFound();
                }

                proyecto.Usuario = user;
                _context.Add(proyecto);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Esta es una notificación en tiempo real");
                TempData["AlertMessage"] = "Proyecto creado exitosamente";
                return RedirectToAction("Index");
            }
            return View(proyecto);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Proyecto proyecto = await _context.Proyectos
                .Include(p => p.Usuario)
                .Include(p => p.Tareas)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (proyecto == null)
            {
                return NotFound();
            }

            return View(proyecto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }

            return View(proyecto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Proyecto proyecto)
        {
            if (ModelState.IsValid)
            {
                _context.Update(proyecto);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Proyecto actualizado exitosamente";
                return RedirectToAction("Index");
            }
            return View(proyecto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Proyectos == null)
            {
                return NotFound();
            }

            var proyecto = await _context.Proyectos
               .FirstOrDefaultAsync(m => m.Id == id);

            if (proyecto == null)
            {
                return NotFound();
            }

            _context.Proyectos.Remove(proyecto);
            await _context.SaveChangesAsync();
            TempData["AlertMessage"] = "Proyecto eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddTarea(int? id)
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }

            Tarea tarea = new()
            {
                FechaRegistro = DateTime.Now,
                Estado = Estado.Pendiente,
                Proyecto = proyecto,
                ProyectoId = proyecto.Id,
                FechaVencimiento = DateTime.Now.AddDays(10)
            };

            return View(tarea);
        }

        [HttpPost]
        public async Task<IActionResult> AddTarea(Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                tarea.Id = 0;
                _context.Add(tarea);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Tarea creada exitosamente";
                return RedirectToAction("Index");
            }
            return View(tarea);
        }

        public async Task<IActionResult> EditTarea(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }

            Tarea model = new()
            {
                Nombre = tarea.Nombre,
                Descripcion = tarea.Descripcion,
                FechaRegistro = DateTime.Now,
                FechaVencimiento = tarea.FechaVencimiento,
                Estado = tarea.Estado,
                Prioridad = tarea.Prioridad,
                ProyectoId = tarea.ProyectoId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditTarea(Tarea model)
        {
            if (ModelState.IsValid)
            {
                Tarea nuevaTarea = new()
                {
                    Id = model.Id,
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    FechaRegistro = DateTime.Now,
                    FechaVencimiento = model.FechaVencimiento,
                    Estado = model.Estado,
                    Prioridad = model.Prioridad,
                    ProyectoId = model.ProyectoId
                };

                _context.Update(nuevaTarea);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Tarea actualizada exitosamente";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteTarea(int? id)
        {
            if (id == null || _context.Tareas == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas
               .FirstOrDefaultAsync(m => m.Id == id);

            if (tarea == null)
            {
                return NotFound();
            }

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();
            TempData["AlertMessage"] = "Tarea eliminada exitosamente";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CompletarTarea(int? id)
        {
            if (id == null || _context.Tareas == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas
               .FirstOrDefaultAsync(m => m.Id == id);

            if (tarea == null)
            {
                return NotFound();
            }

            tarea.Estado = Estado.Completada;

            _context.Tareas.Update(tarea);
            await _context.SaveChangesAsync();
            TempData["AlertMessage"] = "La tarea ha sido marcada como completada";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DetailsTarea(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tarea tarea = await _context.Tareas
                .FirstOrDefaultAsync(s => s.Id == id);

            if (tarea == null)
            {
                return NotFound();
            }

            return View(tarea);
        }
    }
}