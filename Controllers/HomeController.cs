 using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shusha_project_BackUp.Data;
using Shusha_project_BackUp.Data.Migrations;
using Shusha_project_BackUp.DTOs;
using Shusha_project_BackUp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shusha_project_BackUp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; 
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
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
                int TotalRevenus = (int)(totalAmount + totalRestAmount+ total_trader + total_herdSales);
                int centerRevenues = (int)(totalAmount + totalRestAmount);
                ViewBag.CenterRevenues = centerRevenues;    
              //  int TotalRevenus = (int)(totalAmount + totalRestAmount );
                var formattedData = chartData.Select(d => new
                {
                    centerName = d.CenterName,
                    Amount = d.Amount,
                    RestAmount = d.RestAmount,
                    TotalAmount = totalAmount,        // Add total sum of Amount
                    TotalRestAmount = totalRestAmount,
                    TotalRevenues = TotalRevenus

                }).ToList();

                // Pass the chart data to the view
                ViewBag.ChartData = formattedData;

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
                    {   CenterName = centerName,
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
