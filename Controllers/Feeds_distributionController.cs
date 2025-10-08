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
    public class Feeds_distributionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Feeds_distributionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Feeds_distribution
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public async Task<IActionResult> Index()
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            IQueryable<Feeds_distribution> applicationDbContext = _context.Feeds_Distributions.Include(f => f.ward);

            if (userRole == "Magary")
            {
                applicationDbContext = applicationDbContext.Where(d => d.ward.branchId == 1);
            }
            else if (userRole == "Lohman")
            {
                applicationDbContext = applicationDbContext.Where(d => d.ward.branchId == 2);
            }
            else if (userRole == "Bayad A")
            {
                applicationDbContext = applicationDbContext.Where(d => d.ward.branchId == 3);
            }
            else if (userRole == "Bayad B")
            {
                applicationDbContext = applicationDbContext.Where(d => d.ward.branchId == 4);
            }
            else if (userRole == "Admin" || userRole == "LocalAdmin")
            {
                // No filtering for Admin or LocalAdmin; this is the default case
            }



            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Feeds_distribution/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feeds_distribution = await _context.Feeds_Distributions
                .Include(f => f.ward)
                .FirstOrDefaultAsync(m => m.dis_Id == id);
            if (feeds_distribution == null)
            {
                return NotFound();
            }

            return View(feeds_distribution);
        }

        // GET: Feeds_distribution/Create
        public IActionResult Create()
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            // Base query for Wards with Include for branch
            var wardsQuery = _context.Wards.Include(w => w.branch).AsQueryable();

            // Apply branch-specific filtering based on the role
            if (userRole == "Magary")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 1);
            }
            else if (userRole == "Lohman")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 2);
            }
            else if (userRole == "Bayad B")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 3 || w.branchId == 4); // Corrected logical condition
            }

            ViewBag.WardID = new SelectList(wardsQuery, "Ward_ID", "WardName");
            return View();
        }

        // POST: Feeds_distribution/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("dis_Id,quantity,WardID,Date")] Feeds_distribution feeds_distribution)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feeds_distribution);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            // Base query for Wards with Include for branch
            var wardsQuery = _context.Wards.Include(w => w.branch).AsQueryable();

            // Apply branch-specific filtering based on the role
            if (userRole == "Magary")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 1);
            }
            else if (userRole == "Lohman")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 2);
            }
            else if (userRole == "Bayad B")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 3 || w.branchId == 4); // Corrected logical condition
            }

            ViewBag.WardID = new SelectList(wardsQuery, "Ward_ID", "WardName");
            return View(feeds_distribution);
        }

        // GET: Feeds_distribution/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feeds_distribution = await _context.Feeds_Distributions.FindAsync(id);
            if (feeds_distribution == null)
            {
                return NotFound();
            }
            var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            // Base query for Wards with Include for branch
            var wardsQuery = _context.Wards.Include(w => w.branch).AsQueryable();

            // Apply branch-specific filtering based on the role
            if (userRole == "Magary")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 1);
            }
            else if (userRole == "Lohman")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 2);
            }
            else if (userRole == "Bayad B")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 3 || w.branchId == 4); // Corrected logical condition
            }

            ViewBag.WardID = new SelectList(wardsQuery, "Ward_ID", "WardName");
            return View(feeds_distribution);
        }

        // POST: Feeds_distribution/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("dis_Id,quantity,WardID,Date")] Feeds_distribution feeds_distribution)
        {
            if (id != feeds_distribution.dis_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feeds_distribution);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Feeds_distributionExists(feeds_distribution.dis_Id))
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
            var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            // Base query for Wards with Include for branch
            var wardsQuery = _context.Wards.Include(w => w.branch).AsQueryable();

            // Apply branch-specific filtering based on the role
            if (userRole == "Magary")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 1);
            }
            else if (userRole == "Lohman")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 2);
            }
            else if (userRole == "Bayad B")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 3 || w.branchId == 4); // Corrected logical condition
            }

            ViewBag.WardID = new SelectList(wardsQuery, "Ward_ID", "WardName");
            return View(feeds_distribution);
        }

        // GET: Feeds_distribution/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feeds_distribution = await _context.Feeds_Distributions
                .Include(f => f.ward)
                .FirstOrDefaultAsync(m => m.dis_Id == id);
            if (feeds_distribution == null)
            {
                return NotFound();
            }

            return View(feeds_distribution);
        }

        // POST: Feeds_distribution/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feeds_distribution = await _context.Feeds_Distributions.FindAsync(id);
            if (feeds_distribution != null)
            {
                _context.Feeds_Distributions.Remove(feeds_distribution);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Feeds_distributionExists(int id)
        {
            return _context.Feeds_Distributions.Any(e => e.dis_Id == id);
        }
    }
}
