using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shusha_project_BackUp.Data;
using Shusha_project_BackUp.Services;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;

namespace Shusha_project_BackUp.Controllers
{
    [Authorize(Roles = "Admin,proceeds,Accountant")]
    public class Proceeds_TotalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBudgetService _budgetService;

        public Proceeds_TotalsController(ApplicationDbContext context, IBudgetService budgetService)
        {
            _context = context;
            _budgetService = budgetService;
        }

        // GET: Proceeds_Totals
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: API endpoint for DataTables
        [HttpGet]
        public async Task<IActionResult> GetData(DateTime? fromDate, DateTime? toDate, int? month)
        {
            var query = _context.proceeds_Totals.AsQueryable();

            // Apply filters
            if (fromDate.HasValue && toDate.HasValue)
            {
                var from = DateOnly.FromDateTime(fromDate.Value);
                var to = DateOnly.FromDateTime(toDate.Value);
                query = query.Where(p => p.Date >= from && p.Date <= to);
            }
            else if (month.HasValue && month.Value >= 1 && month.Value <= 12)
            {
                var currentYear = DateTime.Now.Year;
                query = query.Where(p => p.Date.Month == month.Value && p.Date.Year == currentYear);
            }

            var data = await query.OrderByDescending(p => p.Date).ToListAsync();

            return Json(new { data });
        }

        // GET: Proceeds_Totals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proceeds_Totals = await _context.proceeds_Totals
                .FirstOrDefaultAsync(m => m.id == id);

            if (proceeds_Totals == null)
            {
                return NotFound();
            }

            return View(proceeds_Totals);
        }

        // GET: Proceeds_Totals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Proceeds_Totals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Date,Egg,broken_Egg,double_Egg,herd,Waste,waste_fees,Miscellaneous")] Proceeds_Totals proceeds_Totals)
        {
            if (ModelState.IsValid)
            {
                proceeds_Totals.UpdateTotal();
                _context.Add(proceeds_Totals);
                await _context.SaveChangesAsync();

                // Update budget automatically
                await _budgetService.UpdateBudgetAsync(proceeds_Totals.Date);

                TempData["SuccessMessage"] = "تم إضافة السجل بنجاح وتحديث الميزانية";
                return RedirectToAction(nameof(Index));
            }
            return View(proceeds_Totals);
        }

        // GET: Proceeds_Totals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proceeds_Totals = await _context.proceeds_Totals.FindAsync(id);
            if (proceeds_Totals == null)
            {
                return NotFound();
            }
            return View(proceeds_Totals);
        }

        // POST: Proceeds_Totals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Date,Egg,broken_Egg,double_Egg,herd,Waste,waste_fees,Miscellaneous")] Proceeds_Totals proceeds_Totals)
        {
            if (id != proceeds_Totals.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    proceeds_Totals.UpdateTotal();
                    _context.Update(proceeds_Totals);
                    await _context.SaveChangesAsync();

                    // Update budget automatically
                    await _budgetService.UpdateBudgetAsync(proceeds_Totals.Date);

                    TempData["SuccessMessage"] = "تم تحديث السجل بنجاح وتحديث الميزانية";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Proceeds_TotalsExists(proceeds_Totals.id))
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
            return View(proceeds_Totals);
        }

        // GET: Proceeds_Totals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proceeds_Totals = await _context.proceeds_Totals
                .FirstOrDefaultAsync(m => m.id == id);

            if (proceeds_Totals == null)
            {
                return NotFound();
            }

            return View(proceeds_Totals);
        }

        // POST: Proceeds_Totals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proceeds_Totals = await _context.proceeds_Totals.FindAsync(id);
            if (proceeds_Totals != null)
            {
                var date = proceeds_Totals.Date;
                _context.proceeds_Totals.Remove(proceeds_Totals);
                await _context.SaveChangesAsync();

                // Update budget automatically after deletion
                await _budgetService.UpdateBudgetAsync(date);

                TempData["SuccessMessage"] = "تم حذف السجل بنجاح وتحديث الميزانية";
            }

            return RedirectToAction(nameof(Index));
        }

        // Export to Excel
        [HttpGet]
        public async Task<IActionResult> ExportToExcel(DateTime? fromDate, DateTime? toDate, int? month)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var query = _context.proceeds_Totals.AsQueryable();

            // Apply filters
            if (fromDate.HasValue && toDate.HasValue)
            {
                var from = DateOnly.FromDateTime(fromDate.Value);
                var to = DateOnly.FromDateTime(toDate.Value);
                query = query.Where(p => p.Date >= from && p.Date <= to);
            }
            else if (month.HasValue && month.Value >= 1 && month.Value <= 12)
            {
                var currentYear = DateTime.Now.Year;
                query = query.Where(p => p.Date.Month == month.Value && p.Date.Year == currentYear);
            }

            var data = await query.OrderBy(p => p.Date).ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("إجمالي الإيرادات");

                // Add title
                worksheet.Cells[1, 1, 1, 10].Merge = true;
                worksheet.Cells[1, 1].Value = "تقرير إجمالي الإيرادات";
                worksheet.Cells[1, 1].Style.Font.Size = 18;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(102, 126, 234));
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Row(1).Height = 35;

                // Add date info
                worksheet.Cells[2, 1, 2, 10].Merge = true;
                if (month.HasValue)
                {
                    var monthName = System.Globalization.CultureInfo.GetCultureInfo("ar-EG").DateTimeFormat.GetMonthName(month.Value);
                    worksheet.Cells[2, 1].Value = $"تقرير شهر: {monthName}";
                }
                else if (fromDate.HasValue && toDate.HasValue)
                {
                    worksheet.Cells[2, 1].Value = $"من {fromDate.Value:yyyy-MM-dd} إلى {toDate.Value:yyyy-MM-dd}";
                }
                else
                {
                    worksheet.Cells[2, 1].Value = $"تقرير السنة: {DateTime.Now.Year}";
                }
                worksheet.Cells[2, 1].Style.Font.Size = 12;
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[2, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(108, 117, 125));
                worksheet.Cells[2, 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Row(2).Height = 25;

                // Add headers
                int headerRow = 4;
                worksheet.Cells[headerRow, 1].Value = "التاريخ";
                worksheet.Cells[headerRow, 2].Value = "البيض";
                worksheet.Cells[headerRow, 3].Value = "البيض المكسور";
                worksheet.Cells[headerRow, 4].Value = "البيض المزدوج";
                worksheet.Cells[headerRow, 5].Value = "القطيع";
                worksheet.Cells[headerRow, 6].Value = "المخلفات";
                worksheet.Cells[headerRow, 7].Value = "رسوم المخلفات";
                worksheet.Cells[headerRow, 8].Value = "متنوعات";
                worksheet.Cells[headerRow, 9].Value = "الإجمالي";

                // Format header row
                using (var range = worksheet.Cells[headerRow, 1, headerRow, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(13, 110, 253));
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Font.Size = 12;
                }

                // Add data rows
                int row = headerRow + 1;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 2].Value = item.Egg;
                    worksheet.Cells[row, 3].Value = item.broken_Egg;
                    worksheet.Cells[row, 4].Value = item.double_Egg;
                    worksheet.Cells[row, 5].Value = item.herd;
                    worksheet.Cells[row, 6].Value = item.Waste;
                    worksheet.Cells[row, 7].Value = item.waste_fees;
                    worksheet.Cells[row, 8].Value = item.Miscellaneous;
                    worksheet.Cells[row, 9].Value = item.total;

                    // Format numbers
                    for (int col = 2; col <= 9; col++)
                    {
                        worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0.00";
                    }

                    // Alternate row colors
                    if ((row - headerRow) % 2 == 0)
                    {
                        using (var rowRange = worksheet.Cells[row, 1, row, 9])
                        {
                            rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rowRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(248, 249, 250));
                        }
                    }

                    row++;
                }

                // Add summary row
                int summaryRow = row;
                worksheet.Cells[summaryRow, 1].Value = "الإجمالي الكلي";
                worksheet.Cells[summaryRow, 2].Value = data.Sum(d => d.Egg);
                worksheet.Cells[summaryRow, 3].Value = data.Sum(d => d.broken_Egg);
                worksheet.Cells[summaryRow, 4].Value = data.Sum(d => d.double_Egg);
                worksheet.Cells[summaryRow, 5].Value = data.Sum(d => d.herd);
                worksheet.Cells[summaryRow, 6].Value = data.Sum(d => d.Waste);
                worksheet.Cells[summaryRow, 7].Value = data.Sum(d => d.waste_fees);
                worksheet.Cells[summaryRow, 8].Value = data.Sum(d => d.Miscellaneous);
                worksheet.Cells[summaryRow, 9].Value = data.Sum(d => d.total);

                // Format summary row
                using (var range = worksheet.Cells[summaryRow, 1, summaryRow, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(25, 135, 84));
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.Font.Size = 12;
                }

                // Format summary numbers
                for (int col = 2; col <= 9; col++)
                {
                    worksheet.Cells[summaryRow, col].Style.Numberformat.Format = "#,##0.00";
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set minimum column width
                for (int col = 1; col <= 9; col++)
                {
                    if (worksheet.Column(col).Width < 15)
                        worksheet.Column(col).Width = 15;
                }

                // Right-to-left for Arabic
                worksheet.View.RightToLeft = true;

                // Add borders to all cells
                var allCells = worksheet.Cells[headerRow, 1, summaryRow, 9];
                allCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                // Center align all data
                worksheet.Cells[headerRow + 1, 1, summaryRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[headerRow + 1, 1, summaryRow, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"Proceeds_Totals_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        private bool Proceeds_TotalsExists(int id)
        {
            return _context.proceeds_Totals.Any(e => e.id == id);
        }
    }
}

