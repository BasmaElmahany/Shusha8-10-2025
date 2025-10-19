using System.Diagnostics;
using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Shusha_project_BackUp.DTOs;
using Shusha_project_BackUp.Models;
using Shusha_project_BackUp.Data;
using Shusha_project_BackUp.Services;

namespace Shusha_project_BackUp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBudgetService _budgetService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBudgetService budgetService)
        {
            _logger = logger;
            _context = context;
            _budgetService = budgetService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                //مبيعات القطيع 
                var total_herdSales = _context.HerdSales.Sum(h => h.total_request_proceed);
                // مبيعات التجار
                var total_trader = _context.Total_Traders.Sum(c => c.Total_traders);

                ViewBag.total_herdSales = total_herdSales;
                ViewBag.Total_trader = total_trader;

                var chartData = await GetDataAsync();
                Console.WriteLine($"Data Count: {chartData}");

                if (chartData == null || !chartData.Any())
                {
                    // Handle the case when no data is returned
                    ViewBag.ErrorMessage = "لا يوجد بيانات";
                    return View();
                }

                // Format the chart data and compute totals
                int totalAmount = (int)chartData.Sum(d => d.Amount);
                int totalRestAmount = (int)chartData.Sum(d => d.RestAmount);
                int TotalRevenus = (int)(totalAmount + totalRestAmount + total_trader + total_herdSales);
                int centerRevenues = (int)(totalAmount + totalRestAmount);
                ViewBag.CenterRevenues = centerRevenues;

                var formattedData = chartData.Select(d => new
                {
                    centerName = d.CenterName,
                    Amount = d.Amount,
                    RestAmount = d.RestAmount,
                    TotalAmount = totalAmount,
                    TotalRestAmount = totalRestAmount,
                    TotalRevenues = TotalRevenus
                }).ToList();

                // Pass the chart data to the view
                ViewBag.ChartData = formattedData;

                // ========== NEW: Budget Year-over-Year Comparison ==========
                var today = DateOnly.FromDateTime(DateTime.Today);
                var currentFiscalYear = _budgetService.GetFiscalYear(today);
                var previousFiscalYear = currentFiscalYear - 1;

                // Get current and previous year budgets
                var currentYearBudget = await _context.Budget
                    .FirstOrDefaultAsync(b => b.year == currentFiscalYear);

                var previousYearBudget = await _context.Budget
                    .FirstOrDefaultAsync(b => b.year == previousFiscalYear);

                // Budget data for current year
                if (currentYearBudget != null)
                {
                    ViewBag.CurrentYear = currentFiscalYear;
                    ViewBag.CurrentYearEgg = currentYearBudget.egg;
                    ViewBag.CurrentYearWaste = currentYearBudget.waste;
                    ViewBag.CurrentYearHerd = currentYearBudget.herd;
                    ViewBag.CurrentYearMiscellaneous = currentYearBudget.Miscellaneous;
                    ViewBag.CurrentYearTotal = currentYearBudget.total;
                }
                else
                {
                    ViewBag.CurrentYear = currentFiscalYear;
                    ViewBag.CurrentYearEgg = 0;
                    ViewBag.CurrentYearWaste = 0;
                    ViewBag.CurrentYearHerd = 0;
                    ViewBag.CurrentYearMiscellaneous = 0;
                    ViewBag.CurrentYearTotal = 0;
                }

                // Budget data for previous year
                if (previousYearBudget != null)
                {
                    ViewBag.PreviousYear = previousFiscalYear;
                    ViewBag.PreviousYearEgg = previousYearBudget.egg;
                    ViewBag.PreviousYearWaste = previousYearBudget.waste;
                    ViewBag.PreviousYearHerd = previousYearBudget.herd;
                    ViewBag.PreviousYearMiscellaneous = previousYearBudget.Miscellaneous;
                    ViewBag.PreviousYearTotal = previousYearBudget.total;
                }
                else
                {
                    ViewBag.PreviousYear = previousFiscalYear;
                    ViewBag.PreviousYearEgg = 0;
                    ViewBag.PreviousYearWaste = 0;
                    ViewBag.PreviousYearHerd = 0;
                    ViewBag.PreviousYearMiscellaneous = 0;
                    ViewBag.PreviousYearTotal = 0;
                }

                // Calculate differences and percentage changes
                if (currentYearBudget != null && previousYearBudget != null)
                {
                    ViewBag.EggDifference = currentYearBudget.egg - previousYearBudget.egg;
                    ViewBag.WasteDifference = currentYearBudget.waste - previousYearBudget.waste;
                    ViewBag.HerdDifference = currentYearBudget.herd - previousYearBudget.herd;
                    ViewBag.MiscellaneousDifference = currentYearBudget.Miscellaneous - previousYearBudget.Miscellaneous;
                    ViewBag.TotalDifference = currentYearBudget.total - previousYearBudget.total;

                    // Percentage changes
                    ViewBag.EggPercentage = CalculatePercentageChange(previousYearBudget.egg, currentYearBudget.egg);
                    ViewBag.WastePercentage = CalculatePercentageChange(previousYearBudget.waste, currentYearBudget.waste);
                    ViewBag.HerdPercentage = CalculatePercentageChange(previousYearBudget.herd, currentYearBudget.herd);
                    ViewBag.MiscellaneousPercentage = CalculatePercentageChange(previousYearBudget.Miscellaneous, currentYearBudget.Miscellaneous);
                    ViewBag.TotalPercentage = CalculatePercentageChange(previousYearBudget.total, currentYearBudget.total);
                }
                else
                {
                    ViewBag.EggDifference = 0;
                    ViewBag.WasteDifference = 0;
                    ViewBag.HerdDifference = 0;
                    ViewBag.MiscellaneousDifference = 0;
                    ViewBag.TotalDifference = 0;
                    ViewBag.EggPercentage = 0;
                    ViewBag.WastePercentage = 0;
                    ViewBag.HerdPercentage = 0;
                    ViewBag.MiscellaneousPercentage = 0;
                    ViewBag.TotalPercentage = 0;
                }

                // Check if budget data exists
                ViewBag.HasBudgetData = currentYearBudget != null || previousYearBudget != null;

                return View();
            }
            catch (Exception ex)
            {
                // Log the error and return a friendly error message
                Console.WriteLine($"An error occurred: {ex.Message}");
                ViewBag.ErrorMessage = "خطأ في التحميل";
                return View();
            }
        }

        private decimal CalculatePercentageChange(decimal oldValue, decimal newValue)
        {
            if (oldValue == 0)
                return newValue > 0 ? 100 : 0;

            return ((newValue - oldValue) / oldValue) * 100;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<List<ProceedsWithCenterDto>> GetDataAsync()
        {
            try
            {
                var data = new List<ProceedsDto>();

                // Execute the stored procedure
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "proceeds_depts";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Open the connection if not already open
                    if (command.Connection.State != System.Data.ConnectionState.Open)
                    {
                        await command.Connection.OpenAsync();
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Map each row to a ProceedsDto object
                            var proceedsDto = new ProceedsDto
                            {
                                centerId = reader.GetInt32(reader.GetOrdinal("centerId")),
                                amount = reader.GetDecimal(reader.GetOrdinal("amount")),
                                rest_amount = reader.GetDecimal(reader.GetOrdinal("rest_amount"))
                            };

                            data.Add(proceedsDto);
                        }
                    }
                }

                // Fetch the list of centers
                var centers = await _context.Centers
                    .Select(c => new { c.centerId, c.centerName })
                    .ToListAsync();

                // Map the data to ProceedsWithCenterDto
                var finalData = data.Select(item =>
                {
                    var centerName = centers.FirstOrDefault(c => c.centerId == item.centerId)?.centerName ?? "Unknown";
                    return new ProceedsWithCenterDto
                    {
                        CenterName = centerName,
                        Amount = item.amount,
                        RestAmount = item.rest_amount
                    };
                }).ToList();

                return finalData;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Return an empty list if an error occurs
                return new List<ProceedsWithCenterDto>();
            }
        }
    }
}

