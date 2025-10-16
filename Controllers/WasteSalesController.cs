using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shusha_project_BackUp.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shusha_project_BackUp.Controllers
{
    [Authorize(Roles = "Admin,proceeds,Accountant")]

    public class WasteSalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WasteSalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WasteSales
        public async Task<IActionResult> Index(DateTime? from, DateTime? to, int? month)
        {
            var wasteSales = _context.Waste_Sales.AsQueryable();

            // Apply filters
            if (from.HasValue)
            {
                var fromDate = DateOnly.FromDateTime(from.Value);
                wasteSales = wasteSales.Where(w => w.date >= fromDate);
                ViewBag.From = from.Value.ToString("yyyy-MM-dd");
            }

            if (to.HasValue)
            {
                var toDate = DateOnly.FromDateTime(to.Value);
                wasteSales = wasteSales.Where(w => w.date <= toDate);
                ViewBag.To = to.Value.ToString("yyyy-MM-dd");
            }

            if (month.HasValue && month.Value is >= 1 and <= 12)
            {
                wasteSales = wasteSales.Where(w => w.date.Month == month.Value);
                ViewBag.Month = month.Value;
            }

            var salesList = await wasteSales
                 .OrderByDescending(w => w.date)
                .Include(w => w.branch)
                .ToListAsync();

            // Statistics
            var today = DateOnly.FromDateTime(DateTime.Today);
            var todaySales = salesList.Where(w => w.date == today).ToList();

            ViewBag.TodayTotalMeters = todaySales.Sum(w => w.wasteMeters);
            ViewBag.TodayTotalRevenue = todaySales.Sum(w => w.wasteMeters * w.price);
            ViewBag.TodayTotalFees = todaySales.Sum(w => w.wasteFees);
            ViewBag.TodayTransactions = todaySales.Count;

            ViewBag.TotalMeters = salesList.Sum(w => w.wasteMeters);
            ViewBag.TotalRevenue = salesList.Sum(w => w.wasteMeters * w.price);
            ViewBag.TotalFees = salesList.Sum(w => w.wasteFees);
            ViewBag.TotalTransactions = salesList.Count;
        

            return View(salesList);
        }
        // GET: WasteSales/Details/5
        // GET: WasteSales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var wasteSale = await _context.Waste_Sales
                .Include(w => w.branch)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (wasteSale == null) return NotFound();

            return View(wasteSale);
        }

        // GET: WasteSales/Create
        public IActionResult Create()
        {
            ViewBag.DefaultDate = DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name");
            return View();
        }
        // POST: WasteSales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,wasteMeters,wasteFees,price,TraderName,date,branchId")] Waste_Sales wasteSale)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wasteSale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DefaultDate = wasteSale.date.ToString("yyyy-MM-dd");
            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name", wasteSale.branchId);
            return View(wasteSale);
        }

        // GET: WasteSales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var wasteSale = await _context.Waste_Sales.FindAsync(id);
            if (wasteSale == null) return NotFound();

            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name", wasteSale.branchId);
            return View(wasteSale);
        }

        // POST: WasteSales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,wasteMeters,wasteFees,price,TraderName,date,branchId")] Waste_Sales wasteSale)
        {
            if (id != wasteSale.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wasteSale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WasteSaleExists(wasteSale.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BranchID = new SelectList(_context.Branches, "branch_id", "branch_name", wasteSale.branchId);
            return View(wasteSale);
        }

        // GET: WasteSales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var wasteSale = await _context.Waste_Sales
                .Include(w => w.branch)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (wasteSale == null) return NotFound();

            return View(wasteSale);
        }

        // POST: WasteSales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wasteSale = await _context.Waste_Sales.FindAsync(id);
            if (wasteSale != null)
            {
                _context.Waste_Sales.Remove(wasteSale);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Export to Excel
        public async Task<IActionResult> ExportToExcel(DateTime? from, DateTime? to, int? month)
        {
            var wasteSales = _context.Waste_Sales.Include(c=>c.branch).AsQueryable();

            // Apply same filters as Index
            if (from.HasValue)
            {
                var fromDate = DateOnly.FromDateTime(from.Value);
                wasteSales = wasteSales.Where(w => w.date >= fromDate).Include(c => c.branch);
            }

            if (to.HasValue)
            {
                var toDate = DateOnly.FromDateTime(to.Value);
                wasteSales = wasteSales.Where(w => w.date <= toDate).Include(c => c.branch);
            }

            if (month.HasValue && month.Value >= 1 && month.Value <= 12)
            {
                wasteSales = wasteSales.Where(w => w.date.Month == month.Value).Include(c => c.branch);
            }

            var salesList = await wasteSales.OrderByDescending(w => w.date).Include(c => c.branch).ToListAsync();

            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Create Excel file
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("مبيعات المخلفات");

                // Add headers
                worksheet.Cells[1, 1].Value = "التاريخ";
                worksheet.Cells[1, 2].Value = "المحطة";
                worksheet.Cells[1, 3].Value = "اسم التاجر";
                worksheet.Cells[1, 4].Value = "عدد الأمتار";
                worksheet.Cells[1, 5].Value = "السعر للمتر";
                worksheet.Cells[1, 6].Value = "اجره تحميل";
                worksheet.Cells[1, 7].Value = "الإجمالي";

                // Format header row
                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(13, 110, 253));
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                // Add data rows
                int row = 2;
                foreach (var sale in salesList)
                {
                    worksheet.Cells[row, 1].Value = sale.date.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 2].Value = sale.branch.branch_name ?? "غير محدد";
                    worksheet.Cells[row, 3].Value = sale.TraderName ?? "غير محدد";
                    worksheet.Cells[row, 4].Value = sale.wasteMeters;
                    worksheet.Cells[row, 5].Value = sale.price;
                    worksheet.Cells[row, 6].Value = sale.wasteFees;
                    worksheet.Cells[row, 7].Value = (sale.wasteMeters * sale.price)+ sale.wasteFees;

                    // Format numbers
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[row, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "#,##0.00";

                    row++;
                }

                // Add summary row
                worksheet.Cells[row, 1].Value = "الإجمالي";
                worksheet.Cells[row, 2].Value = "";
                worksheet.Cells[row, 3].Value = "";
                worksheet.Cells[row, 4].Value = salesList.Sum(w => w.wasteMeters);
                worksheet.Cells[row, 5].Value = salesList.Any() ? salesList.Average(w => w.price) : 0;
                worksheet.Cells[row, 6].Value = salesList.Sum(w => w.wasteFees);
                worksheet.Cells[row, 7].Value = salesList.Sum(w => w.wasteMeters * w.price);

                // Format summary row
                using (var range = worksheet.Cells[row, 1, row, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 53, 69));
                    range.Style.Font.Color.SetColor(Color.White);
                }

                // Format summary numbers
                worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set minimum column width
                for (int col = 1; col <= 7; col++)
                {
                    if (worksheet.Column(col).Width < 15)
                        worksheet.Column(col).Width = 15;
                }

                // Right-to-left for Arabic
                worksheet.View.RightToLeft = true;

                // Add borders to all cells
                var allCells = worksheet.Cells[1, 1, row, 7];
                allCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                // Center align all data
                worksheet.Cells[2, 1, row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, row, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"Waste_Sales_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        private bool WasteSaleExists(int id)
        {
            return _context.Waste_Sales.Any(e => e.Id == id);
        }
    }
}

