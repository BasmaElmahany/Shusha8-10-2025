using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shusha_project_BackUp;
using Shusha_project_BackUp.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using Shusha_project_BackUp.Data.Migrations;

namespace Shusha_project_BackUp.Controllers
{
    public class Water_InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Water_InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Water_Invoices
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Water_Invoices.Include(w => w.branch);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Water_Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterInvoice = await _context.Water_Invoices
                .Include(w => w.branch)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (waterInvoice == null)
            {
                return NotFound();
            }

            return View(waterInvoice);
        }

        // GET: Water_Invoices/Create
        public IActionResult Create()
        {
            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name");
            return View();
        }

        // POST: Water_Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile Invoice_photo, int Amount, int branchID)
        {
            if (ModelState.IsValid)
            {
                if (Invoice_photo != null && Invoice_photo.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", Invoice_photo.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await Invoice_photo.CopyToAsync(stream);
                    }

                    var waterInvoice = new Water_Invoices
                    {
                        Date = DateTime.Now,
                        amount = Amount,
                        Invoice_photo = $"/uploads/{Invoice_photo.FileName}",
                        BranchID = branchID
                    };

                    _context.Water_Invoices.Add(waterInvoice);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name");
            }
            return View();
        }

        // GET: Water_Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterInvoice = await _context.Water_Invoices.FindAsync(id);
            if (waterInvoice == null)
            {
                return NotFound();
            }

            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name", waterInvoice.BranchID);
            return View(waterInvoice);
        }

        // POST: Water_Invoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Amount,Invoice_photo,BranchID")] Water_Invoices waterInvoice, IFormFile Invoice_photo)
        {
            if (id != waterInvoice.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (Invoice_photo != null && Invoice_photo.Length > 0)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", Invoice_photo.FileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await Invoice_photo.CopyToAsync(stream);
                        }

                        waterInvoice.Invoice_photo = $"/uploads/{Invoice_photo.FileName}";
                    }

                    _context.Update(waterInvoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Water_InvoicesExists(waterInvoice.ID))
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
            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name", waterInvoice.BranchID);
            return View(waterInvoice);
        }

        // GET: Water_Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterInvoice = await _context.Water_Invoices
                .Include(w => w.branch)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (waterInvoice == null)
            {
                return NotFound();
            }

            return View(waterInvoice);
        }

        // POST: Water_Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waterInvoice = await _context.Water_Invoices.FindAsync(id);
            if (waterInvoice != null)
            {
                _context.Water_Invoices.Remove(waterInvoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Water_InvoicesExists(int id)
        {
            return _context.Water_Invoices.Any(e => e.ID == id);
        }
    }
}
