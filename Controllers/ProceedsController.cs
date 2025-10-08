using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Shusha_project_BackUp;
using Shusha_project_BackUp.Data;

namespace Shusha_project_BackUp.Controllers
{
    [Authorize(Roles = "Admin,proceeds")]
    public class ProceedsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProceedsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Proceeds
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Proceeds.Include(p => p.center);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Proceeds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proceed = await _context.Proceeds
                .Include(p => p.center)
                .FirstOrDefaultAsync(m => m.id == id);
            if (proceed == null)
            {
                return NotFound();
            }

            return View(proceed);
        }

        // GET: Proceeds/Create
        public IActionResult Create()
        {
            ViewBag.centerId = new SelectList(_context.Centers, "centerId", "centerName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,centerId,amount")] Proceed proceed)
        {
            if (ModelState.IsValid)
            {
                var requested_proceed = await _context.Request_Proceeds.Where(c=>c.centerID==proceed.centerId).FirstOrDefaultAsync();
                if (requested_proceed == null)
                {
                    // Handle the case where requested_proceed is not found
                    ModelState.AddModelError("", "Requested proceed not found.");
                    ViewBag.centerId = new SelectList(_context.Centers, "centerId", "centerName", proceed.centerId);
                    return View(proceed);
                }

                // Ensure the amount is valid
                if (proceed.amount <= 0)
                {
                    ModelState.AddModelError("amount", "Amount must be greater than zero.");
                    ViewBag.centerId = new SelectList(_context.Centers, "centerId", "centerName", proceed.centerId);
                    return View(proceed);
                }

                // Ensure the amount does not exceed requested_proceeds
                if (requested_proceed.requested_proceeds < proceed.amount)
                {
                    ModelState.AddModelError("amount", "Amount cannot exceed the requested proceeds.");
                    ViewBag.centerId = new SelectList(_context.Centers, "centerId", "centerName", proceed.centerId);
                    return View(proceed);
                }

                // Update the requested_proceeds by subtracting the proceed amount
                requested_proceed.requested_proceeds -= proceed.amount;

                // Set the rest_amount
                proceed.rest_amount = requested_proceed.requested_proceeds;

                // Set the current date
                proceed.Date = DateOnly.FromDateTime(DateTime.Now);

                // Add the new proceed record to the context
                _context.Add(proceed);
                await _context.SaveChangesAsync();

                // Redirect to Index view
                return RedirectToAction(nameof(Index));
            }

            // If the model state is not valid, return to the same view
            ViewBag.centerId = new SelectList(_context.Centers, "centerId", "centerName", proceed.centerId);
            return View(proceed);
        }


        // GET: Proceeds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proceed = await _context.Proceeds.FindAsync(id);
            if (proceed == null)
            {
                return NotFound();
            }
            ViewBag.centerId = new SelectList(_context.Centers, "centerId", "centerName", proceed.centerId);
            return View(proceed);
        }

        // POST: Proceeds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,centerId,amount")] Proceed proceed)
        {
            if (id != proceed.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing proceed data from the database
                    var existingProceed = await _context.Proceeds
                                                        .Include(p => p.center) // Include center to calculate requested_proceeds
                                                        .FirstOrDefaultAsync(p => p.id == proceed.id);

                    if (existingProceed == null)
                    {
                        return NotFound();
                    }

                    // Get the corresponding Request_Proceeds for the centerId
                    var requestedProceed = await _context.Request_Proceeds.FindAsync(proceed.centerId);

                    if (requestedProceed != null)
                    {
                        // Adjust requested_proceeds based on the change in amount (add previous amount back)
                        requestedProceed.requested_proceeds += existingProceed.amount; // Add the old amount back
                        requestedProceed.requested_proceeds -= proceed.amount; // Subtract the new amount

                        // Update the rest_amount
                        proceed.rest_amount = requestedProceed.requested_proceeds;
                    }

                    // Update the date to the current date
                    proceed.Date = DateOnly.FromDateTime(DateTime.Now);

                    // Mark the proceed as modified
                    _context.Update(proceed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProceedExists(proceed.id))
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

            // If the model state is invalid, or when the form is re-rendered, update the dropdown for centerId
            ViewBag.centerId = new SelectList(_context.Centers, "centerId", "centerName", proceed.centerId);
            return View(proceed);
        }


        // GET: Proceeds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proceed = await _context.Proceeds
                .Include(p => p.center)
                .FirstOrDefaultAsync(m => m.id == id);
            if (proceed == null)
            {
                return NotFound();
            }

            return View(proceed);
        }

        // POST: Proceeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proceed = await _context.Proceeds
                                        .Include(p => p.center)  // Include center to access its details
                                        .FirstOrDefaultAsync(m => m.id == id);

            if (proceed == null)
            {
                return NotFound();
            }

            // Adjust the requested_proceeds value before deletion
            var requestedProceed = await _context.Request_Proceeds
                                                 .FirstOrDefaultAsync(rp => rp.centerID == proceed.centerId);

            if (requestedProceed != null)
            {
                // Add the amount back to requested_proceeds because the Proceed is being deleted
                requestedProceed.requested_proceeds += proceed.amount;

               

                // Update the Request_Proceeds entity in the database
                _context.Request_Proceeds.Update(requestedProceed);
            }

            // Now, delete the Proceed entity
            _context.Proceeds.Remove(proceed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool ProceedExists(int id)
        {
            return _context.Proceeds.Any(e => e.id == id);
        }

        public async Task<IActionResult> ExportProceedsReportToExcel()
        {
            var reportData = await _context.Proceeds
                .Include(p => p.center)
                .GroupBy(p => new { p.centerId, p.center.centerName })
                .Select(group => new
                {
                    CenterName = group.Key.centerName,
                    TotalProceeds = group.Sum(p => p.amount)
                })
                .ToListAsync();

            if (!reportData.Any())
            {
                TempData["ErrorMessage"] = "لا يوجد بيانات متاحة للتصدير.";
                return RedirectToAction("Index");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("تقرير الإيرادات");

                worksheet.View.RightToLeft = true; // Set Right to Left for Arabic layout

                // Headers
                string[] headers = { "المركز", "الإيرادات الإجمالية" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Data
                int row = 2;
                foreach (var item in reportData)
                {
                    worksheet.Cells[row, 1].Value = item.CenterName ?? "غير محدد";
                    worksheet.Cells[row, 2].Value = item.TotalProceeds;
                    row++;
                }

                worksheet.Cells.AutoFitColumns(); // Auto-fit column widths

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Proceeds_Report_{DateTime.Now:yyyy-MM-dd}.xlsx");
            }
        }
    }

}
