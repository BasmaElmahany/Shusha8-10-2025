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
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBudgetService _budgetService;

        public BudgetController(ApplicationDbContext context, IBudgetService budgetService)
        {
            _context = context;
            _budgetService = budgetService;
        }

        // GET: Budget
        public async Task<IActionResult> Index()
        {
            var budgets = await _context.Budget
                .OrderByDescending(b => b.year)
                .ToListAsync();

            return View(budgets);
        }

        // GET: Budget/Comparison
        public async Task<IActionResult> Comparison(int? currentYear, int? previousYear)
        {
            // Default to current and previous fiscal year
            var today = DateOnly.FromDateTime(DateTime.Today);
            var defaultCurrentYear = _budgetService.GetFiscalYear(today);
            var defaultPreviousYear = defaultCurrentYear - 1;

            currentYear ??= defaultCurrentYear;
            previousYear ??= defaultPreviousYear;

            var comparison = await _budgetService.GetBudgetComparisonAsync(currentYear.Value, previousYear.Value);

            ViewBag.CurrentYear = currentYear.Value;
            ViewBag.PreviousYear = previousYear.Value;
            ViewBag.AvailableYears = await _context.Budget
                .Select(b => b.year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            return View(comparison);
        }

        // GET: Budget/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _context.Budget
                .FirstOrDefaultAsync(m => m.id == id);

            if (budget == null)
            {
                return NotFound();
            }

            // Get fiscal year date range
            var (startDate, endDate) = _budgetService.GetFiscalYearRange(budget.year);
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            // Get all proceeds for this fiscal year
            var proceeds = await _context.proceeds_Totals
                .Where(p => p.Date >= startDate && p.Date <= endDate)
                .OrderBy(p => p.Date)
                .ToListAsync();

            ViewBag.Proceeds = proceeds;

            return View(budget);
        }

        // GET: Budget/Recalculate
        public async Task<IActionResult> Recalculate(int year)
        {
            await _budgetService.RecalculateBudgetForYearAsync(year);
            TempData["SuccessMessage"] = $"تم إعادة حساب ميزانية السنة المالية {year} بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // GET: Budget/RecalculateAll
        public async Task<IActionResult> RecalculateAll()
        {
            var allYears = await _context.Budget
                .Select(b => b.year)
                .Distinct()
                .ToListAsync();

            foreach (var year in allYears)
            {
                await _budgetService.RecalculateBudgetForYearAsync(year);
            }

            TempData["SuccessMessage"] = "تم إعادة حساب جميع الميزانيات بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // Export Budget to Excel
        [HttpGet]
        public async Task<IActionResult> ExportToExcel(int? year)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var query = _context.Budget.AsQueryable();

            if (year.HasValue)
            {
                query = query.Where(b => b.year == year.Value);
            }

            var budgets = await query.OrderBy(b => b.year).ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("الميزانية");

                // Add title
                worksheet.Cells[1, 1, 1, 6].Merge = true;
                worksheet.Cells[1, 1].Value = "تقرير الميزانية السنوية";
                worksheet.Cells[1, 1].Style.Font.Size = 18;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(102, 126, 234));
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Row(1).Height = 35;

                // Add headers
                int headerRow = 3;
                worksheet.Cells[headerRow, 1].Value = "السنة المالية";
                worksheet.Cells[headerRow, 2].Value = "البيض";
                worksheet.Cells[headerRow, 3].Value = "المخلفات";
                worksheet.Cells[headerRow, 4].Value = "القطيع";
                worksheet.Cells[headerRow, 5].Value = "متنوعات";
                worksheet.Cells[headerRow, 6].Value = "الإجمالي";

                // Format header row
                using (var range = worksheet.Cells[headerRow, 1, headerRow, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(13, 110, 253));
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Font.Size = 12;
                }

                // Add data rows
                int row = headerRow + 1;
                foreach (var budget in budgets)
                {
                    worksheet.Cells[row, 1].Value = $"{budget.year}/{budget.year + 1}";
                    worksheet.Cells[row, 2].Value = budget.egg;
                    worksheet.Cells[row, 3].Value = budget.waste;
                    worksheet.Cells[row, 4].Value = budget.herd;
                    worksheet.Cells[row, 5].Value = budget.Miscellaneous;
                    worksheet.Cells[row, 6].Value = budget.total;

                    // Format numbers
                    for (int col = 2; col <= 6; col++)
                    {
                        worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0.00";
                    }

                    // Alternate row colors
                    if ((row - headerRow) % 2 == 0)
                    {
                        using (var rowRange = worksheet.Cells[row, 1, row, 6])
                        {
                            rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rowRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(248, 249, 250));
                        }
                    }

                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Right-to-left for Arabic
                worksheet.View.RightToLeft = true;

                // Add borders
                var allCells = worksheet.Cells[headerRow, 1, row - 1, 6];
                allCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"Budget_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        // Export Comparison to Excel
        [HttpGet]
        public async Task<IActionResult> ExportComparisonToExcel(int currentYear, int previousYear)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var comparison = await _budgetService.GetBudgetComparisonAsync(currentYear, previousYear);

            if (comparison == null)
            {
                TempData["ErrorMessage"] = "لا توجد بيانات للمقارنة";
                return RedirectToAction(nameof(Comparison));
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("مقارنة الميزانية");

                // Add title
                worksheet.Cells[1, 1, 1, 5].Merge = true;
                worksheet.Cells[1, 1].Value = $"مقارنة الميزانية: {previousYear}/{previousYear + 1} مع {currentYear}/{currentYear + 1}";
                worksheet.Cells[1, 1].Style.Font.Size = 18;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(102, 126, 234));
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Row(1).Height = 35;

                // Add headers
                int headerRow = 3;
                worksheet.Cells[headerRow, 1].Value = "البند";
                worksheet.Cells[headerRow, 2].Value = $"السنة السابقة ({previousYear})";
                worksheet.Cells[headerRow, 3].Value = $"السنة الحالية ({currentYear})";
                worksheet.Cells[headerRow, 4].Value = "الفرق";
                worksheet.Cells[headerRow, 5].Value = "نسبة التغيير %";

                // Format header row
                using (var range = worksheet.Cells[headerRow, 1, headerRow, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(13, 110, 253));
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Font.Size = 12;
                }

                // Add data rows
                int row = headerRow + 1;

                // Egg row
                worksheet.Cells[row, 1].Value = "البيض";
                worksheet.Cells[row, 2].Value = comparison.PreviousBudget.egg;
                worksheet.Cells[row, 3].Value = comparison.CurrentBudget.egg;
                worksheet.Cells[row, 4].Value = comparison.EggDifference;
                worksheet.Cells[row, 5].Value = comparison.EggPercentageChange;
                row++;

                // Waste row
                worksheet.Cells[row, 1].Value = "المخلفات";
                worksheet.Cells[row, 2].Value = comparison.PreviousBudget.waste;
                worksheet.Cells[row, 3].Value = comparison.CurrentBudget.waste;
                worksheet.Cells[row, 4].Value = comparison.WasteDifference;
                worksheet.Cells[row, 5].Value = comparison.WastePercentageChange;
                row++;

                // Herd row
                worksheet.Cells[row, 1].Value = "القطيع";
                worksheet.Cells[row, 2].Value = comparison.PreviousBudget.herd;
                worksheet.Cells[row, 3].Value = comparison.CurrentBudget.herd;
                worksheet.Cells[row, 4].Value = comparison.HerdDifference;
                worksheet.Cells[row, 5].Value = comparison.HerdPercentageChange;
                row++;

                // Miscellaneous row
                worksheet.Cells[row, 1].Value = "متنوعات";
                worksheet.Cells[row, 2].Value = comparison.PreviousBudget.Miscellaneous;
                worksheet.Cells[row, 3].Value = comparison.CurrentBudget.Miscellaneous;
                worksheet.Cells[row, 4].Value = comparison.MiscellaneousDifference;
                worksheet.Cells[row, 5].Value = comparison.MiscellaneousPercentageChange;
                row++;

                // Total row
                worksheet.Cells[row, 1].Value = "الإجمالي";
                worksheet.Cells[row, 2].Value = comparison.PreviousBudget.total;
                worksheet.Cells[row, 3].Value = comparison.CurrentBudget.total;
                worksheet.Cells[row, 4].Value = comparison.TotalDifference;
                worksheet.Cells[row, 5].Value = comparison.TotalPercentageChange;

                // Format total row
                using (var range = worksheet.Cells[row, 1, row, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(25, 135, 84));
                    range.Style.Font.Color.SetColor(Color.White);
                }

                // Format numbers
                for (int r = headerRow + 1; r <= row; r++)
                {
                    for (int col = 2; col <= 4; col++)
                    {
                        worksheet.Cells[r, col].Style.Numberformat.Format = "#,##0.00";
                    }
                    worksheet.Cells[r, 5].Style.Numberformat.Format = "0.00%";
                }

                // Color code differences
                for (int r = headerRow + 1; r <= row; r++)
                {
                    var diffValue = (decimal)worksheet.Cells[r, 4].Value;
                    if (diffValue > 0)
                    {
                        worksheet.Cells[r, 4].Style.Font.Color.SetColor(Color.Green);
                    }
                    else if (diffValue < 0)
                    {
                        worksheet.Cells[r, 4].Style.Font.Color.SetColor(Color.Red);
                    }
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Right-to-left for Arabic
                worksheet.View.RightToLeft = true;

                // Add borders
                var allCells = worksheet.Cells[headerRow, 1, row, 5];
                allCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"Budget_Comparison_{previousYear}_vs_{currentYear}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}

