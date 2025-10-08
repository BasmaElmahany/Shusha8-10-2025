using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class Request_ProceedsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Request_ProceedsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Request_Proceeds
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Request_Proceeds.Include(r => r.center);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Request_Proceeds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request_Proceeds = await _context.Request_Proceeds
                .Include(r => r.center)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request_Proceeds == null)
            {
                return NotFound();
            }

            return View(request_Proceeds);
        }

        // GET: Request_Proceeds/Create
        public IActionResult Create()
        {
            ViewBag.centerID = new SelectList(_context.Centers, "centerId", "centerName");
            return View();
        }

        // POST: Request_Proceeds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,centerID,requested_proceeds")] Request_Proceeds request_Proceeds)
        {
            if (ModelState.IsValid)
            {
                _context.Add(request_Proceeds);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.centerID = new SelectList(_context.Centers, "centerId", "centerName", request_Proceeds.centerID);
            return View(request_Proceeds);
        }

        // GET: Request_Proceeds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request_Proceeds = await _context.Request_Proceeds.FindAsync(id);
            if (request_Proceeds == null)
            {
                return NotFound();
            }
            ViewBag.centerID = new SelectList(_context.Centers, "centerId", "centerName", request_Proceeds.centerID);
            return View(request_Proceeds);
        }

        // POST: Request_Proceeds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,centerID,requested_proceeds")] Request_Proceeds request_Proceeds)
        {
            if (id != request_Proceeds.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request_Proceeds);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Request_ProceedsExists(request_Proceeds.Id))
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
            ViewBag.centerID = new SelectList(_context.Centers, "centerId", "centerName", request_Proceeds.centerID);
            return View(request_Proceeds);
        }

        // GET: Request_Proceeds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request_Proceeds = await _context.Request_Proceeds
                .Include(r => r.center)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request_Proceeds == null)
            {
                return NotFound();
            }

            return View(request_Proceeds);
        }

        // POST: Request_Proceeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request_Proceeds = await _context.Request_Proceeds.FindAsync(id);
            if (request_Proceeds != null)
            {
                _context.Request_Proceeds.Remove(request_Proceeds);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Request_ProceedsExists(int id)
        {
            return _context.Request_Proceeds.Any(e => e.Id == id);
        }

        public IActionResult ExportToExcel()
        {
            var proceedsData = _context.Request_Proceeds
                .Include(r => r.center)
                .ToList();

            if (!proceedsData.Any())
            {
                TempData["ErrorMessage"] = "لا يوجد بيانات متاحة للتصدير.";
                return RedirectToAction("Index");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("طلبات الإيرادات");
                worksheet.View.RightToLeft = true;

                // Headers
                string[] headers = { "المركز", "الإيرادات المطلوبة", "تاريخ الطلب" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cells[1, i + 1];
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                int row = 2;
                foreach (var item in proceedsData)
                {
                    worksheet.Cells[row, 1].Value = item.center?.centerName ?? "غير محدد";  // Column 1
                    worksheet.Cells[row, 2].Value = item.requested_proceeds;                 // Column 2
                    worksheet.Cells[row, 3].Value = DateTime.Now.ToString("yyyy-MM-dd", new CultureInfo("ar-EG"));

                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Request_Proceeds_{DateTime.Now:yyyy-MM-dd}.xlsx");
            }
        }
    }
}
