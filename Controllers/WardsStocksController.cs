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
    public class WardsStocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WardsStocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WardsStocks
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.wardsStocks.Include(w => w.ward);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WardsStocks/Details/5
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wardsStock = await _context.wardsStocks
                .Include(w => w.ward)
                .FirstOrDefaultAsync(m => m.stockID == id);
            if (wardsStock == null)
            {
                return NotFound();
            }

            return View(wardsStock);
        }

        // GET: WardsStocks/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.wardID = new SelectList(_context.Wards, "Ward_ID", "WardName");
            return View();
        }

        // POST: WardsStocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("stockID,whiteEggs,rest_whEggs,brownEggs,rest_brEggs,brokenEggs,rest_bkEggs,doubleEggs,rest_dbEggs,wardID,Date")] WardsStock wardsStock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wardsStock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.wardID = new SelectList(_context.Wards, "Ward_ID", "WardName", wardsStock.wardID);
            return View(wardsStock);
        }

        // GET: WardsStocks/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wardsStock = await _context.wardsStocks.FindAsync(id);
            if (wardsStock == null)
            {
                return NotFound();
            }
            ViewBag.wardID = new SelectList(_context.Wards, "Ward_ID", "WardName", wardsStock.wardID);
            return View(wardsStock);
        }

        // POST: WardsStocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("stockID,whiteEggs,rest_whEggs,brownEggs,rest_brEggs,brokenEggs,rest_bkEggs,doubleEggs,rest_dbEggs,wardID,Date")] WardsStock wardsStock)
        {
            if (id != wardsStock.stockID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wardsStock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WardsStockExists(wardsStock.stockID))
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
            ViewBag.wardID = new SelectList(_context.Wards, "Ward_ID", "WardName", wardsStock.wardID);
            return View(wardsStock);
        }

        // GET: WardsStocks/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wardsStock = await _context.wardsStocks
                .Include(w => w.ward)
                .FirstOrDefaultAsync(m => m.stockID == id);
            if (wardsStock == null)
            {
                return NotFound();
            }

            return View(wardsStock);
        }

        // POST: WardsStocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wardsStock = await _context.wardsStocks.FindAsync(id);
            if (wardsStock != null)
            {
                _context.wardsStocks.Remove(wardsStock);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WardsStockExists(int id)
        {
            return _context.wardsStocks.Any(e => e.stockID == id);
        }
    }
}
