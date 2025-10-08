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
    public class MedicineUsedsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicineUsedsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MedicineUseds
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.medicineUsed.Include(m => m.Medicine);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MedicineUseds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicineUsed = await _context.medicineUsed
                .Include(m => m.Medicine)
                .FirstOrDefaultAsync(m => m.id == id);
            if (medicineUsed == null)
            {
                return NotFound();
            }

            return View(medicineUsed);
        }

        // GET: MedicineUseds/Create
        public IActionResult Create()
        {
            ViewData["MedicineID"] = new SelectList(_context.medicines, "ID", "Description");
            return View();
        }

        // POST: MedicineUseds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,purpose,quantityUsed,Date,MedicineID")] MedicineUsed medicineUsed)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicineUsed);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicineID"] = new SelectList(_context.medicines, "ID", "Description", medicineUsed.MedicineID);
            return View(medicineUsed);
        }

        // GET: MedicineUseds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicineUsed = await _context.medicineUsed.FindAsync(id);
            if (medicineUsed == null)
            {
                return NotFound();
            }
            ViewData["MedicineID"] = new SelectList(_context.medicines, "ID", "Description", medicineUsed.MedicineID);
            return View(medicineUsed);
        }

        // POST: MedicineUseds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,purpose,quantityUsed,Date,MedicineID")] MedicineUsed medicineUsed)
        {
            if (id != medicineUsed.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicineUsed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicineUsedExists(medicineUsed.id))
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
            ViewData["MedicineID"] = new SelectList(_context.medicines, "ID", "Description", medicineUsed.MedicineID);
            return View(medicineUsed);
        }

        // GET: MedicineUseds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicineUsed = await _context.medicineUsed
                .Include(m => m.Medicine)
                .FirstOrDefaultAsync(m => m.id == id);
            if (medicineUsed == null)
            {
                return NotFound();
            }

            return View(medicineUsed);
        }

        // POST: MedicineUseds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicineUsed = await _context.medicineUsed.FindAsync(id);
            if (medicineUsed != null)
            {
                _context.medicineUsed.Remove(medicineUsed);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicineUsedExists(int id)
        {
            return _context.medicineUsed.Any(e => e.id == id);
        }
    }
}
