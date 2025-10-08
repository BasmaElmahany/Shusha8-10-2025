using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shusha_project_BackUp;
using Shusha_project_BackUp.Data;

namespace Shusha_project_BackUp.Controllers
{
    [Authorize(Roles = "Admin,Accountant")]
    public class Egg_stockController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Egg_stockController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Egg_stock
        public async Task<IActionResult> Index()
        {
            return View(await _context.Egg_stock.ToListAsync());
        }

        // GET: Egg_stock/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var egg_stock = await _context.Egg_stock
                .FirstOrDefaultAsync(m => m.Id == id);
            if (egg_stock == null)
            {
                return NotFound();
            }

            return View(egg_stock);
        }

        // GET: Egg_stock/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Egg_stock/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,plates_whiteEggs,plates_BrownEggs,brokenPlates,doubleEggs")] Egg_stock egg_stock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(egg_stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(egg_stock);
        }

        // GET: Egg_stock/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var egg_stock = await _context.Egg_stock.FindAsync(id);
            if (egg_stock == null)
            {
                return NotFound();
            }
            return View(egg_stock);
        }

        // POST: Egg_stock/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,plates_whiteEggs,plates_BrownEggs,brokenPlates,doubleEggs,total")] Egg_stock egg_stock)
        {
            if (id != egg_stock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(egg_stock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Egg_stockExists(egg_stock.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(egg_stock);
        }

        // GET: Egg_stock/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var egg_stock = await _context.Egg_stock
                .FirstOrDefaultAsync(m => m.Id == id);
            if (egg_stock == null)
            {
                return NotFound();
            }

            return View(egg_stock);
        }

        // POST: Egg_stock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var egg_stock = await _context.Egg_stock.FindAsync(id);
            if (egg_stock != null)
            {
                _context.Egg_stock.Remove(egg_stock);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Egg_stockExists(int id)
        {
            return _context.Egg_stock.Any(e => e.Id == id);
        }
    }
}
