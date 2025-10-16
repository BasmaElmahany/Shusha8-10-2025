using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shusha_project_BackUp.Data;
using Shusha_project_BackUp.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;

namespace Shusha_project_BackUp.Controllers
{
    [Authorize(Roles = "Admin,proceeds,Accountant")]
    public class SalesReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SalesReport
        public IActionResult Index()
        {
            return View();
        }

        // Export Sales Data to Excel
        public async Task<IActionResult> ExportSalesDataExcel(DateTime? from, DateTime? to, int? month)
        {
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            List<ExcelMonthData> monthlyData = new List<ExcelMonthData>();

            if (month.HasValue && month.Value >= 1 && month.Value <= 12)
            {
                // Single month report
                var data = await GetMonthData(month.Value);
                monthlyData.Add(data);
            }
            else if (from.HasValue && to.HasValue)
            {
                // Date range report - group by month
                var startDate = DateOnly.FromDateTime(from.Value);
                var endDate = DateOnly.FromDateTime(to.Value);

                // Get all months in the range
                var months = new List<int>();
                var currentDate = startDate;
                while (currentDate <= endDate)
                {
                    if (!months.Contains(currentDate.Month))
                    {
                        months.Add(currentDate.Month);
                    }
                    currentDate = currentDate.AddMonths(1);
                }

                foreach (var m in months.OrderBy(x => x))
                {
                    var data = await GetMonthData(m, startDate, endDate);
                    monthlyData.Add(data);
                }
            }
            else
            {
                // All months in current year
                var currentYear = DateTime.Now.Year;
                for (int m = 1; m <= 12; m++)
                {
                    var data = await GetMonthData(m);
                    if (data.Total > 0) // Only include months with data
                    {
                        monthlyData.Add(data);
                    }
                }
            }

            // Create Excel file
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("تقرير المبيعات الشامل");

                // Add title
                worksheet.Cells[1, 1, 1, 5].Merge = true;
                worksheet.Cells[1, 1].Value = "تقرير المبيعات الشامل";
                worksheet.Cells[1, 1].Style.Font.Size = 18;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(52, 58, 64));
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Row(1).Height = 35;

                // Add date info
                worksheet.Cells[2, 1, 2, 5].Merge = true;
                if (month.HasValue)
                {
                    var monthName = System.Globalization.CultureInfo.GetCultureInfo("ar-EG").DateTimeFormat.GetMonthName(month.Value);
                    worksheet.Cells[2, 1].Value = $"تقرير شهر: {monthName}";
                }
                else if (from.HasValue && to.HasValue)
                {
                    worksheet.Cells[2, 1].Value = $"من {from.Value:yyyy-MM-dd} إلى {to.Value:yyyy-MM-dd}";
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
                worksheet.Cells[headerRow, 1].Value = "الشهر";
                worksheet.Cells[headerRow, 2].Value = "مبيعات البيض";
                worksheet.Cells[headerRow, 3].Value = "مبيعات المخلفات";
                worksheet.Cells[headerRow, 4].Value = "مبيعات القطيع";
                worksheet.Cells[headerRow, 5].Value = "الإجمالي";

                // Format header row
                using (var range = worksheet.Cells[headerRow, 1, headerRow, 5])
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
                foreach (var data in monthlyData)
                {
                    var monthName = System.Globalization.CultureInfo.GetCultureInfo("ar-EG").DateTimeFormat.GetMonthName(data.month);

                    worksheet.Cells[row, 1].Value = monthName;
                    worksheet.Cells[row, 2].Value = data.total_traderSales;
                    worksheet.Cells[row, 3].Value = data.total_wasteSales;
                    worksheet.Cells[row, 4].Value = data.total_HerdSales;
                    worksheet.Cells[row, 5].Value = data.Total;

                    // Format numbers
                    worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";

                    // Alternate row colors
                    if ((row - headerRow) % 2 == 0)
                    {
                        using (var rowRange = worksheet.Cells[row, 1, row, 5])
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
                worksheet.Cells[summaryRow, 2].Value = monthlyData.Sum(d => d.total_traderSales);
                worksheet.Cells[summaryRow, 3].Value = monthlyData.Sum(d => d.total_wasteSales);
                worksheet.Cells[summaryRow, 4].Value = monthlyData.Sum(d => d.total_HerdSales);
                worksheet.Cells[summaryRow, 5].Value = monthlyData.Sum(d => d.Total);

                // Format summary row
                using (var range = worksheet.Cells[summaryRow, 1, summaryRow, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(25, 135, 84));
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.Font.Size = 12;
                }

                // Format summary numbers
                worksheet.Cells[summaryRow, 2].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[summaryRow, 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[summaryRow, 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[summaryRow, 5].Style.Numberformat.Format = "#,##0.00";

                // Add breakdown section
                int breakdownStartRow = summaryRow + 3;
                worksheet.Cells[breakdownStartRow, 1, breakdownStartRow, 5].Merge = true;
                worksheet.Cells[breakdownStartRow, 1].Value = "تفصيل النسب المئوية";
                worksheet.Cells[breakdownStartRow, 1].Style.Font.Size = 14;
                worksheet.Cells[breakdownStartRow, 1].Style.Font.Bold = true;
                worksheet.Cells[breakdownStartRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[breakdownStartRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[breakdownStartRow, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(108, 117, 125));
                worksheet.Cells[breakdownStartRow, 1].Style.Font.Color.SetColor(Color.White);

                double grandTotal = monthlyData.Sum(d => d.Total);
                double traderTotal = monthlyData.Sum(d => d.total_traderSales);
                double wasteTotal = monthlyData.Sum(d => d.total_wasteSales);
                double herdTotal = monthlyData.Sum(d => d.total_HerdSales);

                int percentRow = breakdownStartRow + 2;

                // Egg sales percentage
                worksheet.Cells[percentRow, 2].Value = "مبيعات البيض:";
                worksheet.Cells[percentRow, 3].Value = grandTotal > 0 ? (traderTotal / grandTotal * 100) : 0;
                worksheet.Cells[percentRow, 3].Style.Numberformat.Format = "0.00\"%\"";
                worksheet.Cells[percentRow, 4].Value = traderTotal;
                worksheet.Cells[percentRow, 4].Style.Numberformat.Format = "#,##0.00";

                percentRow++;

                // Waste sales percentage
                worksheet.Cells[percentRow, 2].Value = "مبيعات المخلفات:";
                worksheet.Cells[percentRow, 3].Value = grandTotal > 0 ? (wasteTotal / grandTotal * 100) : 0;
                worksheet.Cells[percentRow, 3].Style.Numberformat.Format = "0.00\"%\"";
                worksheet.Cells[percentRow, 4].Value = wasteTotal;
                worksheet.Cells[percentRow, 4].Style.Numberformat.Format = "#,##0.00";

                percentRow++;

                // Herd sales percentage
                worksheet.Cells[percentRow, 2].Value = "مبيعات القطيع:";
                worksheet.Cells[percentRow, 3].Value = grandTotal > 0 ? (herdTotal / grandTotal * 100) : 0;
                worksheet.Cells[percentRow, 3].Style.Numberformat.Format = "0.00\"%\"";
                worksheet.Cells[percentRow, 4].Value = herdTotal;
                worksheet.Cells[percentRow, 4].Style.Numberformat.Format = "#,##0.00";

                // Style breakdown section
                using (var range = worksheet.Cells[breakdownStartRow + 2, 2, percentRow, 4])
                {
                    range.Style.Font.Size = 11;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set minimum column width
                for (int col = 1; col <= 5; col++)
                {
                    if (worksheet.Column(col).Width < 20)
                        worksheet.Column(col).Width = 20;
                }

                // Right-to-left for Arabic
                worksheet.View.RightToLeft = true;

                // Add borders to all cells
                var allCells = worksheet.Cells[headerRow, 1, summaryRow, 5];
                allCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                // Center align all data
                worksheet.Cells[headerRow + 1, 1, summaryRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[headerRow + 1, 1, summaryRow, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"Sales_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        private async Task<ExcelMonthData> GetMonthData(int month, DateOnly? startDate = null, DateOnly? endDate = null)
        {
            var currentYear = DateTime.Now.Year;

            // Trader Sales (Egg Sales)
            var traderSales = _context.TraderSales.AsQueryable();
            traderSales = traderSales.Where(t => t.date.Month == month && t.date.Year == currentYear);

            if (startDate.HasValue)
                traderSales = traderSales.Where(t => t.date >= startDate.Value);
            if (endDate.HasValue)
                traderSales = traderSales.Where(t => t.date <= endDate.Value);

            var totalTraderSales = await traderSales.SumAsync(t => t.RequestProceed);

            // Herd Sales
            var herdSales = _context.HerdSales.AsQueryable();
            herdSales = herdSales.Where(h => h.Date.Month == month && h.Date.Year == currentYear);

            if (startDate.HasValue)
                herdSales = herdSales.Where(h => h.Date >= startDate.Value);
            if (endDate.HasValue)
                herdSales = herdSales.Where(h => h.Date <= endDate.Value);

            var totalHerdSales = await herdSales.SumAsync(h => h.total_request_proceed);

            // Waste Sales
            var wasteSales = _context.Waste_Sales.AsQueryable();
            wasteSales = wasteSales.Where(w => w.date.Month == month && w.date.Year == currentYear);

            if (startDate.HasValue)
                wasteSales = wasteSales.Where(w => w.date >= startDate.Value);
            if (endDate.HasValue)
                wasteSales = wasteSales.Where(w => w.date <= endDate.Value);

            var totalWasteSales = await wasteSales.SumAsync(w => (double)(w.wasteMeters * w.price));

            return new ExcelMonthData
            {
                month = month,
                total_traderSales = (double)totalTraderSales,
                total_HerdSales = totalHerdSales,
                total_wasteSales = totalWasteSales,
                Total = (double)totalTraderSales + totalHerdSales + totalWasteSales
            };
        }
    }
}

