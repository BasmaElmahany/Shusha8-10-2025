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
    [Authorize(Roles = "Admin,Accountant")]
    public class Solar_InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Solar_InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Solar_Invoices
        public async Task<IActionResult> Index()
        {
            var solarInvoices = _context.Solar_Invoices.Include(s => s.branch);
            return View(await solarInvoices.ToListAsync());
        }

        // GET: Solar_Invoices/Create
        public IActionResult Create()
        {
            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name");
            return View();
        }

        // POST: Solar_Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("invoice_id,amount,Date,BranchID")] Solar_Invoices solar_Invoices)
        {
            if (ModelState.IsValid)
            {
                _context.Add(solar_Invoices);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name", solar_Invoices.BranchID);
            return View(solar_Invoices);
        }

        // GET: Solar_Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solar_Invoices = await _context.Solar_Invoices.FindAsync(id);
            if (solar_Invoices == null)
            {
                return NotFound();
            }
            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name", solar_Invoices.BranchID);
            return View(solar_Invoices);
        }

        // POST: Solar_Invoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("invoice_id,amount,Date,BranchID")] Solar_Invoices solar_Invoices)
        {
            if (id != solar_Invoices.invoice_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(solar_Invoices);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Solar_InvoicesExists(solar_Invoices.invoice_id))
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
            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name", solar_Invoices.BranchID);
            return View(solar_Invoices);
        }

        // GET: Solar_Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solar_Invoices = await _context.Solar_Invoices
                .FirstOrDefaultAsync(m => m.invoice_id == id);
            if (solar_Invoices == null)
            {
                return NotFound();
            }

            return View(solar_Invoices);
        }

        // POST: Solar_Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var solar_Invoices = await _context.Solar_Invoices.FindAsync(id);
            if (solar_Invoices != null)
            {
                _context.Solar_Invoices.Remove(solar_Invoices);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Solar_InvoicesExists(int id)
        {
            return _context.Solar_Invoices.Any(e => e.invoice_id == id);
        }
    }
}
