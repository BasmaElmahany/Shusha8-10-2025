using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Shusha_project_BackUp;

namespace Shusha_project_BackUp.Controllers
{
    [Authorize(Roles = "Admin,Accountant")]
    public class Eelectricity_InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Eelectricity_InvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Eelectricity_Invoice
        public async Task<IActionResult> Index()
        {
            var data = await _context.Electricity_Invoices.Include(i=>i.branch).ToListAsync();
            return View(data);
        }

        
        // GET: Eelectricity_Invoice/Create
        public IActionResult Create()
        {
            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name");
            return View();
        }

        // POST: Eelectricity_Invoice/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile Invoice_photo, int Amount , int branchID)
        {
            if (ModelState.IsValid)
            {
                if (Invoice_photo != null || Invoice_photo.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", Invoice_photo.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await Invoice_photo.CopyToAsync(stream);
                    }
                    var Eelectricity_Invoice = new Eelectricity_Invoice
                    {
                        Date = DateTime.Now,
                        amount = Amount,
                        Invoice_photo= $"/uploads/{Invoice_photo.FileName}",
                        BranchID = branchID
                    };
                    _context.Electricity_Invoices.Add(Eelectricity_Invoice);
                   await _context.SaveChangesAsync();
                    return RedirectToAction("Index");

                }
                ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name");
            }
            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var invoice = await _context.Electricity_Invoices.FindAsync(id);
            if (invoice == null) return NotFound();

            // Populate the SelectList for BranchID
            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name", invoice.BranchID);
            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,amount,Invoice_photo,BranchID")] Eelectricity_Invoice invoice, IFormFile Invoice_photo)
        {
            if (id != invoice.ID) return NotFound();

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
                        invoice.Invoice_photo = $"/uploads/{Invoice_photo.FileName}";
                    }

                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.ID))
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

            // Repopulate the SelectList if model validation fails
            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name", invoice.BranchID);
            return View(invoice);
        }


        // Details Action
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var invoice = await _context.Electricity_Invoices
                .FirstOrDefaultAsync(m => m.ID == id);
            if (invoice == null) return NotFound();

            return View(invoice);
        }

        // Delete Action
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var invoice = await _context.Electricity_Invoices
                .FirstOrDefaultAsync(m => m.ID == id);
            if (invoice == null) return NotFound();

            return View(invoice);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Electricity_Invoices.FindAsync(id);
            _context.Electricity_Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.Electricity_Invoices.Any(e => e.ID == id);
        }

    }
}
