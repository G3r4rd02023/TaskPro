using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskPro.Data;
using TaskPro.Data.Entidades;

namespace TaskPro.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ProyectosController : Controller
    {
        private readonly DataContext _context;

        public ProyectosController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Proyectos.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Proyecto proyecto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(proyecto);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Proyecto creado exitosamente";
                return RedirectToAction("Index");
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
    }
}