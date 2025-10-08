using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Shusha_project_BackUp;
using Shusha_project_BackUp.Data;
using Shusha_project_BackUp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shusha_project_BackUp.Controllers
{

    public class Daily_ProductionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Daily_ProductionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Daily_Production
        /*public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Daily_Productions.Include(d => d.ward);
            return View(await applicationDbContext.ToListAsync());
        }*/
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B,Accountant")]
        public async Task<IActionResult> Index()
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            IQueryable<DailySales> dailyProductionsQuery = _context.Daily_Productions.Include(d => d.ward);

            // Apply filtering based on user role
            if (userRole == "Magary")
                dailyProductionsQuery = dailyProductionsQuery.Where(d => d.ward.branchId == 1);
            else if (userRole == "Lohman")
                dailyProductionsQuery = dailyProductionsQuery.Where(d => d.ward.branchId == 2);
            else if (userRole == "Bayad A")
                dailyProductionsQuery = dailyProductionsQuery.Where(d => d.ward.branchId == 3);
            else if (userRole == "Bayad B")
                dailyProductionsQuery = dailyProductionsQuery.Where(d => d.ward.branchId == 4);
            else if (userRole == "Admin" || userRole == "LocalAdmin")
            {
                // No filtering for Admin or LocalAdmin
            }

            var dailyProductions = await dailyProductionsQuery.ToListAsync();

            // Calculate totals
            var totalWhiteEggs = dailyProductions.Sum(d => d.No_of_Wheggs);
            var totalBrownEggs = dailyProductions.Sum(d => d.No_of_Breggs);
            var totalDoubleEggs = dailyProductions.Sum(d => d.double_eggs);
            var totalBrokenEggs = dailyProductions.Sum(d => d.no_of_carton_broken);

            // Save to totalProduction_vm table
            var totalProduction = new totalProduction_vm
            {
                total_whiteEggs = (int)totalWhiteEggs,
                total_brownEggs = (int)totalBrownEggs,
                total_doubleEggs = (int)totalDoubleEggs,
                total_brokenEggs = (int)totalBrokenEggs
            };

            _context.totalProduction_Vms.Add(totalProduction);
            await _context.SaveChangesAsync();
            ViewBag.TotalWhiteEggs = totalWhiteEggs;
            ViewBag.TotalBrownEggs = totalBrownEggs;
            ViewBag.TotalDoubleEggs = totalDoubleEggs;
            ViewBag.TotalBrokenEggs = totalBrokenEggs;
            return View(dailyProductions);
        }




        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B,Accountant")]
        // GET: Daily_Production/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daily_Production = await _context.Daily_Productions
                .Include(d => d.ward)
                .FirstOrDefaultAsync(m => m.id == id);
            if (daily_Production == null)
            {
                return NotFound();
            }

            return View(daily_Production);
        }

        // GET: Daily_Production/Create
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public IActionResult Create()
        {
            // Get the current user's role
            var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            // Base query for Wards with Include for branch
            var wardsQuery = _context.Wards.Include(w => w.branch).AsQueryable();

            // Apply branch-specific filtering based on the role
            if (userRole == "Magary")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 1);
            }
            else if (userRole == "Lohman")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 2);
            }
            else if (userRole == "Bayad B")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 3 || w.branchId == 4); // Corrected logical condition
            }

            // No filtering for Admin

            // Populate the filtered SelectList
            ViewBag.ward_id = new SelectList(wardsQuery.ToList(), "Ward_ID", "WardName");

            return View();
        }




        // POST: Daily_Production/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public async Task<IActionResult> Create([Bind("id,ward_id,No_of_Wheggs,No_of_Breggs,Whdead_herd,Brdead_herd,no_of_carton_broken,double_eggs,Date")] DailySales daily_Production)
        {
            if (ModelState.IsValid)
            {
                // Set the Date value
                //daily_Production.Date = DateOnly.FromDateTime(DateTime.Now);

                // Check if a record already exists with the same ward_id and Date
                var existingRecord = await _context.Daily_Productions
                    .FirstOrDefaultAsync(dp => dp.ward_id == daily_Production.ward_id && dp.Date == daily_Production.Date);

                if (existingRecord != null)
                {
                    // Return an error message or handle duplicate scenario
                    ModelState.AddModelError(string.Empty, "عفوا يوجد تسجيل لنفس العنبر لهذا اليوم");
                    ViewBag.ward_id = new SelectList(_context.Wards, "Ward_ID", "WardName", daily_Production.ward_id);
                    return View(daily_Production);
                }

                // Fetch the existing stock for the ward
                var wardStock = await _context.wardsStocks
                    .FirstOrDefaultAsync(ws => ws.wardID == daily_Production.ward_id);

                if (wardStock == null)
                {
                    // Initialize a new stock record if it does not exist
                    wardStock = new WardsStock
                    {
                        wardID = (int)daily_Production.ward_id,
                        Date = daily_Production.Date,
                        whiteEggs = (int)daily_Production.No_of_Wheggs,
                        brownEggs = (int)daily_Production.No_of_Breggs,
                        brokenEggs = daily_Production.no_of_carton_broken,
                        doubleEggs = daily_Production.double_eggs,
                        rest_whEggs = 0,
                        rest_brEggs = 0,
                        rest_bkEggs = 0,
                        rest_dbEggs = 0
                    };
                    _context.wardsStocks.Add(wardStock);
                }
                else
                {
                    // Update the existing stock
                    wardStock.whiteEggs += (int)daily_Production.No_of_Wheggs;
                    wardStock.brownEggs += (int)daily_Production.No_of_Breggs;
                    wardStock.brokenEggs += daily_Production.no_of_carton_broken;
                    wardStock.doubleEggs += daily_Production.double_eggs;
                    _context.wardsStocks.Update(wardStock);
                }

                // Update the ward herd numbers for dead herd adjustments
                var WHward = daily_Production.Whdead_herd;
                var Wbr = daily_Production.Brdead_herd;
                var ward = await _context.Wards.FirstOrDefaultAsync(w => w.Ward_ID == daily_Production.ward_id);
                if (ward != null)
                {
                    ward.No_of_Whherd -= WHward;
                    ward.No_of_Brherd -= Wbr;
                    if (ward.No_of_Whherd < 0) ward.No_of_Whherd = 0;
                    if (ward.No_of_Brherd < 0) ward.No_of_Brherd = 0;

                    _context.Update(ward);
                }

                // Add new daily production record
                _context.Daily_Productions.Add(daily_Production);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ward_id = new SelectList(_context.Wards, "Ward_ID", "WardName", daily_Production.ward_id);
            return View(daily_Production);
        }


        [Authorize(Roles = "Admin,Magary")]
        // GET: Daily_Production/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daily_Production = await _context.Daily_Productions.FindAsync(id);
            if (daily_Production == null)
            {
                return NotFound();
            }
            ViewBag.ward_id = new SelectList(_context.Wards, "Ward_ID", "WardName", daily_Production.ward_id);
            return View(daily_Production);
        }

        // POST: Daily_Production/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Magary")]
        public async Task<IActionResult> Edit(int id, [Bind("id,ward_id,No_of_Wheggs,No_of_Breggs,Whdead_herd,Brdead_herd,no_of_carton_broken,double_eggs,Date")] DailySales daily_Production)
        {
            if (id != daily_Production.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the previous production record
                    var previousProduction = await _context.Daily_Productions.AsNoTracking()
                        .Where(dp => dp.id == daily_Production.id)
                        .FirstOrDefaultAsync();

                    // Fetch the stock record for the corresponding ward
                    var wardStock = await _context.wardsStocks
                        .Where(ws => ws.wardID == daily_Production.ward_id )
                        .FirstOrDefaultAsync();

                    if (previousProduction != null && wardStock != null)
                    {
                        // Reverse the previous production values from stock
                        wardStock.whiteEggs -= (int)previousProduction.No_of_Wheggs;
                        wardStock.brownEggs -= (int)previousProduction.No_of_Breggs;
                        wardStock.brokenEggs -= previousProduction.no_of_carton_broken;
                        wardStock.doubleEggs -= previousProduction.double_eggs;

                        // Update rest values after reversing
                        wardStock.rest_whEggs -= (int)previousProduction.No_of_Wheggs;
                        wardStock.rest_brEggs -= (int)previousProduction.No_of_Breggs;
                        wardStock.rest_bkEggs -= previousProduction.no_of_carton_broken;
                        wardStock.rest_dbEggs -= previousProduction.double_eggs;

                        // Add the new production values to the stock
                        wardStock.whiteEggs += (int)daily_Production.No_of_Wheggs;
                        wardStock.brownEggs += (int)daily_Production.No_of_Breggs;
                        wardStock.brokenEggs += daily_Production.no_of_carton_broken;
                        wardStock.doubleEggs += daily_Production.double_eggs;

                        // Update rest values after adding new production
                        wardStock.rest_whEggs += (int)daily_Production.No_of_Wheggs;
                        wardStock.rest_brEggs += (int)daily_Production.No_of_Breggs;
                        wardStock.rest_bkEggs += daily_Production.no_of_carton_broken;
                        wardStock.rest_dbEggs += daily_Production.double_eggs;

                        // Ensure no negative values for stock or rest
                        wardStock.whiteEggs = Math.Max(0, wardStock.whiteEggs);
                        wardStock.brownEggs = Math.Max(0, wardStock.brownEggs);
                        wardStock.brokenEggs = Math.Max(0, wardStock.brokenEggs);
                        wardStock.doubleEggs = Math.Max(0, wardStock.doubleEggs);

                        wardStock.rest_whEggs = Math.Max(0, wardStock.rest_whEggs);
                        wardStock.rest_brEggs = Math.Max(0, wardStock.rest_brEggs);
                        wardStock.rest_bkEggs = Math.Max(0, wardStock.rest_bkEggs);
                        wardStock.rest_dbEggs = Math.Max(0, wardStock.rest_dbEggs);

                        // Update the stock record
                        _context.Update(wardStock);
                    }
                    else
                    {
                        ModelState.AddModelError("", "لا يوجد تسجيلات للمخزون أو الإنتاج");
                        ViewBag.ward_id = new SelectList(_context.Wards, "Ward_ID", "WardName", daily_Production.ward_id);
                        return View(daily_Production);
                    }

                    // Update the production record
                    _context.Update(daily_Production);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Daily_ProductionExists(daily_Production.id))
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

            ViewBag.ward_id = new SelectList(_context.Wards, "Ward_ID", "WardName", daily_Production.ward_id);
            return View(daily_Production);
        }

        [Authorize(Roles = "Admin")]
        // GET: Daily_Production/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daily_Production = await _context.Daily_Productions
                .Include(d => d.ward)
                .FirstOrDefaultAsync(m => m.id == id);
            if (daily_Production == null)
            {
                return NotFound();
            }

            return View(daily_Production);
        }

        // POST: Daily_Production/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var daily_Production = await _context.Daily_Productions.FindAsync(id);
            if (daily_Production != null)
            {
                _context.Daily_Productions.Remove(daily_Production);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Daily_ProductionExists(int id)
        {
            return _context.Daily_Productions.Any(e => e.id == id);
        }

        public async Task<IActionResult> Chart(int? wardId, string xField, List<string> yFields)
        {
            // Fetch Wards for Dropdown
            ViewBag.Wards = new SelectList(_context.Wards, "Ward_ID", "WardName");

            // If parameters are missing, return default data
            if (wardId == null || string.IsNullOrEmpty(xField) || yFields == null || yFields.Count == 0)
            {
                // Default X-axis data (example: "Date")
                var _x = _context.Daily_Productions.Select(d => d.Date).ToList();

                // Default Y-axis fields data (example: hardcoded values)
                var _y = new List<string>
        {
            "Brdead_herd",
            "Whdead_herd",
            "No_of_Wheggs",
            "No_of_Breggs",
            "no_of_carton_broken",
            "double_eggs"
        };

                // Pass example data to the ViewBag
                ViewBag.XField = _x;
                ViewBag.YFields = _y;

                // Example chart data to display when no parameters are selected
                var exampleChartData = new[]
                {
            new { Date = "2024-12-01", No_of_Wheggs = 50, No_of_Breggs = 40, Whdead_herd = 2, Brdead_herd = 1, double_eggs = 5, no_of_carton_broken = 0 },
            new { Date = "2024-12-02", No_of_Wheggs = 60, No_of_Breggs = 45, Whdead_herd = 1, Brdead_herd = 2, double_eggs = 4, no_of_carton_broken = 1 },
            new { Date = "2024-12-03", No_of_Wheggs = 55, No_of_Breggs = 50, Whdead_herd = 3, Brdead_herd = 1, double_eggs = 6, no_of_carton_broken = 2 }
        };

                // Pass serialized chart data to the ViewBag
                ViewBag.ChartData = JsonConvert.SerializeObject(exampleChartData);

                return View();
            }

            // Fetch the Daily_Productions for the selected ward
            var data = await _context.Daily_Productions
                .Where(dp => dp.ward_id == wardId)
                .OrderBy(dp => dp.Date)
                .ToListAsync();

            // Prepare the chart data
            var chartData = new List<object>();

            // Add X and Y axis data
            foreach (var item in data)
            {
                var row = new Dictionary<string, object>
        {
            { xField, item.Date.ToString("yyyy-MM-dd") } // Add X-axis value (date)
        };

                // Loop through each Y-axis field and fetch corresponding data
                foreach (var field in yFields)
                {
                    // Dynamically get property value from Daily_Productions
                    var propertyValue = item.GetType().GetProperty(field)?.GetValue(item, null);

                    // If the property value is null, default it to 0
                    row[field] = propertyValue ?? 0;
                }

                // Add the row to the chart data
                chartData.Add(row);
            }

            // Serialize chart data to pass to the view
            ViewBag.ChartData = JsonConvert.SerializeObject(chartData);

            return View();
        }

    }

 
}
