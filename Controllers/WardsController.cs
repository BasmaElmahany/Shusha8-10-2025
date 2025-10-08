using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Shusha_project_BackUp;

namespace Shusha_project_BackUp.Controllers
{
    public class WardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Wards
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Wards.Include(w => w.branch);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Wards/Details/5
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.Wards
                .Include(w => w.branch)
                .FirstOrDefaultAsync(m => m.Ward_ID == id);
            if (ward == null)
            {
                return NotFound();
            }

            return View(ward);
        }

        // GET: Wards/Create
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public IActionResult Create()
        {
            ViewBag.branchId = new SelectList(_context.Branches, "branch_id", "branch_name");
            return View();
        }


        // POST: Wards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public async Task<IActionResult> Create([Bind("Ward_ID,WardName,No_of_Whherd,No_of_Brherd,branchId")] Ward ward)
        {
            ward.Date = DateOnly.FromDateTime(DateTime.Now);
            if (ModelState.IsValid)
            {
                _context.Add(ward);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            ViewBag.branchId = new SelectList(_context.Branches, "BranchId", "BranchName",ward.branchId);

            return View(ward);
        }

        // GET: Wards/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.Wards.FindAsync(id);
            if (ward == null)
            {
                return NotFound();
            }
            ViewBag.branchId = new SelectList(_context.Branches, "branch_id", "branch_name", ward.branchId);
            return View(ward);
        }


        // POST: Wards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Ward_ID,WardName,No_of_Whherd,No_of_Brherd,branchId")] Ward ward)
        {
            if (id != ward.Ward_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ward);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WardExists(ward.Ward_ID))
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
            ViewBag.branchId = new SelectList(_context.Branches, "branch_id", "branch_name", ward.branchId);
            return View(ward);
        }

        // GET: Wards/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.Wards
                .Include(w => w.branch)
                .FirstOrDefaultAsync(m => m.Ward_ID == id);
            if (ward == null)
            {
                return NotFound();
            }

            return View(ward);
        }

        // POST: Wards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ward = await _context.Wards.FindAsync(id);
            if (ward != null)
            {
                _context.Wards.Remove(ward);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WardExists(int id)
        {
            return _context.Wards.Any(e => e.Ward_ID == id);
        }
        
    }
}
