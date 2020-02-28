using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;

namespace Soccer.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeamsController : Controller
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public TeamsController(DataContext context,
            IConverterHelper converterHelper,
            IImageHelper imageHelper)
        {
            _context = context;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            return View(await _context.Teams.ToListAsync());
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TeamEntity teamEntity = await _context.Teams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teamEntity == null)
            {
                return NotFound();
            }

            return View(teamEntity);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamViewModel teamViewModel)
        {
            if (ModelState.IsValid)
            {
                //Si el modelo es válido creamos un path que contendrá la ruta de la imagen
                //la cual sacamos del modelo através del convertidor
                string path = string.Empty;
                if (teamViewModel.LogoFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(teamViewModel.LogoFile, "Teams");
                }
                //Ahora creamos el entity usando el convertidor y lo guardamos
                TeamEntity teamEntity = _converterHelper.ToTeamEntity(teamViewModel, path, true);
                _context.Add(teamEntity);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    if (e.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"Ya existe un equipo con el nombre: {teamEntity.Name}");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, e.InnerException.Message);
                    }

                }
            }
            return View(teamViewModel);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TeamEntity teamEntity = await _context.Teams.FindAsync(id);
            if (teamEntity == null)
            {
                return NotFound();
            }
            TeamViewModel teamViewModel = _converterHelper.ToTeamViewModel(teamEntity);
            return View(teamViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeamViewModel teamViewModel)
        {
            if (id != teamViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //Para esto se tenia que guardar en el hidden el .path
                string path = teamViewModel.LogoPath;
                if (teamViewModel.LogoFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(teamViewModel.LogoFile, "Teams");
                }
                //Ahora creamos el entity usando el convertidor y lo guardamos pero con false
                TeamEntity teamEntity = _converterHelper.ToTeamEntity(teamViewModel, path, false);
                _context.Update(teamEntity);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    if (e.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"Ya existe un equipo con el nombre{teamEntity.Name}");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, e.InnerException.Message);
                    }

                }
            }
            return View(teamViewModel);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TeamEntity teamEntity = await _context.Teams
                .FirstOrDefaultAsync(m => m.Id == id);

            if (teamEntity == null)
            {
                return NotFound();
            }
            _context.Teams.Remove(teamEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamEntityExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}
