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
    [Authorize(Roles = "Admin,Accountant,proceeds")]
    public class HerdSalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HerdSalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HerdSales
        public async Task<IActionResult> Index()
        {
            return View(await _context.HerdSales.ToListAsync());
        }

        // GET: HerdSales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var herdSales = await _context.HerdSales
                .FirstOrDefaultAsync(m => m.id == id);
            if (herdSales == null)
            {
                return NotFound();
            }

            return View(herdSales);
        }

        // GET: HerdSales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HerdSales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,NO_wh_herd,weight_white,wh_price_kilo,NO_br_herd,weight_brown,br_price_kilo,Date,IsPaid")] HerdSales herdSales)
        {
            if (ModelState.IsValid)
            {
                herdSales.req_total_wh = herdSales.NO_wh_herd * herdSales.weight_white * herdSales.wh_price_kilo;
                herdSales.req_total_br = herdSales.NO_br_herd * herdSales.weight_brown * herdSales.br_price_kilo;
                herdSales.total_request_proceed = herdSales.req_total_wh + herdSales.req_total_br;
                _context.Add(herdSales);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(herdSales);
        }

        // GET: HerdSales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var herdSales = await _context.HerdSales.FindAsync(id);
            if (herdSales == null)
            {
                return NotFound();
            }
            return View(herdSales);
        }

        // POST: HerdSales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,NO_wh_herd,weight_white,wh_price_kilo,NO_br_herd,weight_brown,br_price_kilo,Date,IsPaid")] HerdSales herdSales)
        {
            if (id != herdSales.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRecord = await _context.HerdSales.FirstOrDefaultAsync(h => h.id == id);
                    if (existingRecord != null)
                    {
                        existingRecord.NO_wh_herd = herdSales.NO_wh_herd;
                        existingRecord.weight_white = herdSales.weight_white;
                        existingRecord.wh_price_kilo = herdSales.wh_price_kilo;
                        existingRecord.NO_br_herd = herdSales.NO_br_herd;
                        existingRecord.weight_brown = herdSales.weight_brown;
                        existingRecord.br_price_kilo = herdSales.br_price_kilo;
                        existingRecord.Date = herdSales.Date;
                        existingRecord.IsPaid = herdSales.IsPaid;

                        // Calculate totals
                        existingRecord.req_total_wh = existingRecord.NO_wh_herd * existingRecord.weight_white * existingRecord.wh_price_kilo;
                        existingRecord.req_total_br = existingRecord.NO_br_herd * existingRecord.weight_brown * existingRecord.br_price_kilo;
                        existingRecord.total_request_proceed = existingRecord.req_total_wh + existingRecord.req_total_br;

                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HerdSalesExists(herdSales.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(herdSales);
        }

        // GET: HerdSales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var herdSales = await _context.HerdSales
                .FirstOrDefaultAsync(m => m.id == id);
            if (herdSales == null)
            {
                return NotFound();
            }

            return View(herdSales);
        }

        // POST: HerdSales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var herdSales = await _context.HerdSales.FindAsync(id);
            if (herdSales != null)
            {
                _context.HerdSales.Remove(herdSales);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HerdSalesExists(int id)
        {
            return _context.HerdSales.Any(e => e.id == id);
        }
    }
}
