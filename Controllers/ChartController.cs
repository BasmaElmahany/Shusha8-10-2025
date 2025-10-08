using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shusha_project_BackUp.Data;
using Shusha_project_BackUp.Data.Migrations;
using Shusha_project_BackUp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Shusha_project_BackUp.Controllers
{
    public class ChartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? wardID, int? Month, int? Year)
        {
            try
            {
                // Get all wards for the dropdown list
                var wards = await _context.Wards.Select(w => new { w.Ward_ID, w.WardName }).ToListAsync();
                ViewBag.Wards = new SelectList(wards, "Ward_ID", "WardName");

                // Use default values if no values are provided for wardID, Month, or Year
                if (!wardID.HasValue)
                {
                    wardID = 1;
                    Month = 1;
                    Year = 2025; // Default values (you can adjust this as needed)
                }

                // Fetch chart data based on the selected ward ID
                var chartData = await GetDataAsync(wardID.Value, Month.Value, Year.Value);

                if (chartData == null || !chartData.Any())
                {
                    // Handle the case when no data is returned
                    ViewBag.ErrorMessage = "لا توجد بيانات لعرضها لهذا العنبر في التاريخ المحدد.";
                    return View();
                }

                // Format the chart data for Highcharts
                var formattedData = chartData.Select(d => new
                {
                    Date = d.Date.ToString("yyyy-MM-dd"), // Format the date as a string for the X-axis
                    No_of_Wheggs = d.No_of_Wheggs,
                    No_of_Breggs = d.No_of_Breggs,
                    No_of_Carton_Broken = d.no_of_carton_broken,
                    Double_Eggs = d.double_eggs,
                    Whdead_Herd = d.Whdead_herd,
                    Brdead_Herd = d.Brdead_herd
                }).ToList();

                // Pass the chart data to the view
                ViewBag.ChartData = formattedData;

                // Get the selected ward name
                var selectedWard = await _context.Wards.Where(w => w.Ward_ID == wardID.Value).FirstOrDefaultAsync();
                if (selectedWard != null)
                {
                    ViewBag.SelectedWardID = selectedWard.WardName;
                }
                else
                {
                    ViewBag.SelectedWardID = "غير محدد";
                }

                return View();
            }
            catch (Exception ex)
            {
                // Log the error (optional) and display a user-friendly message
                ViewBag.ErrorMessage = "حدث خطأ أثناء تحميل البيانات. يرجى المحاولة لاحقًا.";
                return View();
            }
        }

        // Execute the stored procedure and map the results to a view model
        public async Task<List<DailySales>> GetDataAsync(int wardID, int? Month, int? Year)
        {
            try
            {
                string sp = "view_Daily_Production"; // Name of your stored procedure

                // Execute the stored procedure and map the result to the view model
                var data = await _context.Daily_Productions
                   .FromSqlRaw($"EXEC {sp} @WardID = {wardID}, @Month = {Month}, @Year = {Year}")
                   .ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                // Log the error (optional) and return an empty list or null
                return null; // Return null if an error occurs
            }
        }
    }
}
