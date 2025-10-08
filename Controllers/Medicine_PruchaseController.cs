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
    public class Medicine_PruchaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Medicine_PruchaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Medicine_Pruchase
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Medicine_Pruchase.Include(m => m.Medicine);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Medicine_Pruchase/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine_Pruchase = await _context.Medicine_Pruchase
                .Include(m => m.Medicine)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (medicine_Pruchase == null)
            {
                return NotFound();
            }

            return View(medicine_Pruchase);
        }

        // GET: Medicine_Pruchase/Create
        public IActionResult Create()
        {
            ViewData["MedicineID"] = new SelectList(_context.medicines, "ID", "Description");
            return View();
        }

        // POST: Medicine_Pruchase/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MedicineID,Quantity,MedicineName,PurchaseDate,Total")] Medicine_Pruchase medicine_Pruchase)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicine_Pruchase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicineID"] = new SelectList(_context.medicines, "ID", "Description", medicine_Pruchase.MedicineID);
            return View(medicine_Pruchase);
        }

        // GET: Medicine_Pruchase/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine_Pruchase = await _context.Medicine_Pruchase.FindAsync(id);
            if (medicine_Pruchase == null)
            {
                return NotFound();
            }
            ViewData["MedicineID"] = new SelectList(_context.medicines, "ID", "Description", medicine_Pruchase.MedicineID);
            return View(medicine_Pruchase);
        }

        // POST: Medicine_Pruchase/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MedicineID,Quantity,MedicineName,PurchaseDate,Total")] Medicine_Pruchase medicine_Pruchase)
        {
            if (id != medicine_Pruchase.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicine_Pruchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Medicine_PruchaseExists(medicine_Pruchase.ID))
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
            ViewData["MedicineID"] = new SelectList(_context.medicines, "ID", "Description", medicine_Pruchase.MedicineID);
            return View(medicine_Pruchase);
        }

        // GET: Medicine_Pruchase/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine_Pruchase = await _context.Medicine_Pruchase
                .Include(m => m.Medicine)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (medicine_Pruchase == null)
            {
                return NotFound();
            }

            return View(medicine_Pruchase);
        }

        // POST: Medicine_Pruchase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicine_Pruchase = await _context.Medicine_Pruchase.FindAsync(id);
            if (medicine_Pruchase != null)
            {
                _context.Medicine_Pruchase.Remove(medicine_Pruchase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Medicine_PruchaseExists(int id)
        {
            return _context.Medicine_Pruchase.Any(e => e.ID == id);
        }
    }
}
