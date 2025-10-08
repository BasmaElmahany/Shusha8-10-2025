using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shusha_project_BackUp;
using Shusha_project_BackUp.Data;

namespace Shusha_project_BackUp.Controllers
{
    public class DailySalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailySalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DailySales
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DailySales.Include<Data.DailySales, Ward>((System.Linq.Expressions.Expression<Func<Data.DailySales, Ward?>>)(d => d.ward));
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DailySales/Details/5
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailySales = await _context.DailySales
                .Include(d => d.ward)
                .FirstOrDefaultAsync(m => m.dailySales_id == id);
            if (dailySales == null)
            {
                return NotFound();
            }

            return View(dailySales);
        }

        // GET: DailySales/Create
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public IActionResult Create()
        { // Get the current user's role
            var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            // Base query for Wards with Include for branch
            var wardsQuery = _context.Wards.Include(w => w.branch).AsQueryable();

            // Apply branch-specific filtration based on the role
            if (userRole == "Magary")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 1);
            }
            if (userRole == "Lohman")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 2);
            }
          
            if (userRole == "Bayad B")
            {
                wardsQuery = wardsQuery.Where(w => w.branchId == 4 || w.branchId==3);
            }
            // No filtering for Admin

            // Populate the filtered SelectList
            ViewBag.wardID = new SelectList(wardsQuery.ToList(), "Ward_ID", "WardName");

            return View();
            
        }

        // POST: DailySales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: DailySales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Magary,Lohman,Bayad A,Bayad B")]
        public async Task<IActionResult> Create([Bind("dailySales_id,wardID,no_of_carton_WhEggs,no_of_carton_BrEggs,no_of_carton_broken,double_eggs,waste_poultry,Date")] Data.DailySales dailySales)
        {
            if (ModelState.IsValid)
            {
             //   dailySales.Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));



                // Fetch the corresponding stock record
                var wardStock = await _context.wardsStocks
                    .Where(ws => ws.wardID == dailySales.wardID )
                    .FirstOrDefaultAsync();

                if (wardStock != null)
                {
                    // Subtract sales quantities from stock
                    wardStock.whiteEggs -= dailySales.no_of_carton_WhEggs;
                    wardStock.brownEggs -= dailySales.no_of_carton_BrEggs;
                    wardStock.brokenEggs -= dailySales.no_of_carton_broken;
                    wardStock.doubleEggs -= dailySales.double_eggs;

                    // Ensure no negative values in stock
                    wardStock.whiteEggs = Math.Max(0, wardStock.whiteEggs);
                    wardStock.brownEggs = Math.Max(0, wardStock.brownEggs);
                    wardStock.brokenEggs = Math.Max(0, wardStock.brokenEggs);
                    wardStock.doubleEggs = Math.Max(0, wardStock.doubleEggs);

                    // Update the rest values based on the remaining stock
                    wardStock.rest_whEggs = wardStock.whiteEggs;
                    wardStock.rest_brEggs = wardStock.brownEggs;
                    wardStock.rest_bkEggs = wardStock.brokenEggs;
                    wardStock.rest_dbEggs = wardStock.doubleEggs;

                    _context.Update(wardStock);
                }
                else
                {
                    ModelState.AddModelError("", "لم يتم العثور على سجل المخزون لهذا اليوم");
                    ViewBag.wardID = new SelectList(_context.Wards, "Ward_ID", "WardName", dailySales.wardID);
                    return View(dailySales);
                }

                // Add the sales record
                _context.Add(dailySales);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.wardID = new SelectList(_context.Wards, "Ward_ID", "WardName", dailySales.wardID);
            return View(dailySales);
        }



        // GET: DailySales/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailySales = await _context.DailySales.FindAsync(id);
            if (dailySales == null)
            {
                return NotFound();
            }
            ViewBag.wardID = new SelectList(_context.Wards, "Ward_ID", "WardName", dailySales.wardID);
            return View(dailySales);
        }

        // POST: DailySales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: DailySales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("dailySales_id,wardID,no_of_carton_WhEggs,no_of_carton_BrEggs,no_of_carton_broken,double_eggs,waste_poultry,Date")] Data.DailySales dailySales)
        {
            if (id != dailySales.dailySales_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the previous sales record
                    var previousSales = await _context.DailySales.AsNoTracking().Include(c=>c.ward)
                        .Where(ds => ds.dailySales_id == dailySales.dailySales_id)
                        .FirstOrDefaultAsync();

                    // Fetch the corresponding production record
                    var dailyProduction = await _context.Daily_Productions.Include(c => c.ward)
                        .Where(dp => dp.ward_id == dailySales.wardID )
                        .FirstOrDefaultAsync();

                    // Fetch the corresponding stock record
                    var wardStock = await _context.wardsStocks
                        .Where(ws => ws.wardID == dailySales.wardID )
                        .FirstOrDefaultAsync();

                    if (dailyProduction != null && wardStock != null)
                    {
                        // Reverse previous sales from production and stock
                        if (previousSales != null)
                        {
                            dailyProduction.No_of_Wheggs += previousSales.no_of_carton_WhEggs;
                            dailyProduction.No_of_Breggs += previousSales.no_of_carton_BrEggs;
                            dailyProduction.no_of_carton_broken += previousSales.no_of_carton_broken;
                            dailyProduction.double_eggs += previousSales.double_eggs;

                            wardStock.whiteEggs += previousSales.no_of_carton_WhEggs;
                            wardStock.brownEggs += previousSales.no_of_carton_BrEggs;
                            wardStock.brokenEggs += previousSales.no_of_carton_broken;
                            wardStock.doubleEggs += previousSales.double_eggs;
                        }

                        // Check if the new sales quantities are valid
                        if (dailySales.no_of_carton_WhEggs > wardStock.whiteEggs ||
                            dailySales.no_of_carton_BrEggs > wardStock.brownEggs ||
                            dailySales.no_of_carton_broken > wardStock.brokenEggs ||
                            dailySales.double_eggs > wardStock.doubleEggs)
                        {
                            ModelState.AddModelError("", "الكمية يجب ان تكون اقل او تساوي الكمية المتاحة");
                            ViewBag.wardID = new SelectList(_context.Wards, "Ward_ID", "WardName", dailySales.wardID);
                            return View(dailySales);
                        }

                        // Subtract new sales quantities from production and stock
                        dailyProduction.No_of_Wheggs -= dailySales.no_of_carton_WhEggs;
                        dailyProduction.No_of_Breggs -= dailySales.no_of_carton_BrEggs;
                        dailyProduction.no_of_carton_broken -= dailySales.no_of_carton_broken;
                        dailyProduction.double_eggs -= dailySales.double_eggs;

                        wardStock.whiteEggs -= dailySales.no_of_carton_WhEggs;
                        wardStock.brownEggs -= dailySales.no_of_carton_BrEggs;
                        wardStock.brokenEggs -= dailySales.no_of_carton_broken;
                        wardStock.doubleEggs -= dailySales.double_eggs;

                        // Ensure no negative stock values
                        wardStock.whiteEggs = Math.Max(0, wardStock.whiteEggs);
                        wardStock.brownEggs = Math.Max(0, wardStock.brownEggs);
                        wardStock.brokenEggs = Math.Max(0, wardStock.brokenEggs);
                        wardStock.doubleEggs = Math.Max(0, wardStock.doubleEggs);

                        // Update rest values
                        wardStock.rest_whEggs = wardStock.whiteEggs;
                        wardStock.rest_brEggs = wardStock.brownEggs;
                        wardStock.rest_bkEggs = wardStock.brokenEggs;
                        wardStock.rest_dbEggs = wardStock.doubleEggs;

                        _context.Update(dailyProduction);
                        _context.Update(wardStock);
                    }
                    else
                    {
                        ModelState.AddModelError("", "لم يتم العثور على تسجيلات الإنتاج أو المخزون");
                        ViewBag.wardID = new SelectList(_context.Wards, "Ward_ID", "WardName", dailySales.wardID);
                        return View(dailySales);
                    }
                    previousSales.Date = dailySales.Date;
                    previousSales.waste_poultry = dailySales.waste_poultry;
                    _context.Update(dailySales);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DailySalesExists(dailySales.dailySales_id))
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

            ViewBag.wardID = new SelectList(_context.Wards, "Ward_ID", "WardName", dailySales.wardID);
            return View(dailySales);
        }


        // GET: DailySales/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailySales = await _context.DailySales
                .Include(d => d.ward)
                .FirstOrDefaultAsync(m => m.dailySales_id == id);
            if (dailySales == null)
            {
                return NotFound();
            }

            return View(dailySales);
        }

        // POST: DailySales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dailySales = await _context.DailySales.FindAsync(id);
            if (dailySales != null)
            {
                // Fetch the previous sales record (if exists)
                var previousSales = await _context.DailySales.AsNoTracking()
                    .Where(ds => ds.dailySales_id == dailySales.dailySales_id)
                    .FirstOrDefaultAsync();

                // Fetch the corresponding production record
                var dailyProduction = await _context.Daily_Productions
                    .Where(dp => dp.ward_id == dailySales.wardID && dp.Date == dailySales.Date)
                    .FirstOrDefaultAsync();

                // Fetch the corresponding stock record
                var wardStock = await _context.wardsStocks
                    .Where(ws => ws.wardID == dailySales.wardID && ws.Date == dailySales.Date)
                    .FirstOrDefaultAsync();

                if (dailyProduction != null && wardStock != null)
                {
                    // Add back the sales quantities to production and stock
                    if (previousSales != null)
                    {
                        dailyProduction.No_of_Wheggs += previousSales.no_of_carton_WhEggs;
                        dailyProduction.No_of_Breggs += previousSales.no_of_carton_BrEggs;
                        dailyProduction.no_of_carton_broken += previousSales.no_of_carton_broken;
                        dailyProduction.double_eggs += previousSales.double_eggs;

                        wardStock.whiteEggs += previousSales.no_of_carton_WhEggs;
                        wardStock.brownEggs += previousSales.no_of_carton_BrEggs;
                        wardStock.brokenEggs += previousSales.no_of_carton_broken;
                        wardStock.doubleEggs += previousSales.double_eggs;
                    }

                    // Ensure no negative stock values
                    wardStock.whiteEggs = Math.Max(0, wardStock.whiteEggs);
                    wardStock.brownEggs = Math.Max(0, wardStock.brownEggs);
                    wardStock.brokenEggs = Math.Max(0, wardStock.brokenEggs);
                    wardStock.doubleEggs = Math.Max(0, wardStock.doubleEggs);

                    // Update rest values
                    wardStock.rest_whEggs = wardStock.whiteEggs;
                    wardStock.rest_brEggs = wardStock.brownEggs;
                    wardStock.rest_bkEggs = wardStock.brokenEggs;
                    wardStock.rest_dbEggs = wardStock.doubleEggs;

                    // Update the production and stock records
                    _context.Update(dailyProduction);
                    _context.Update(wardStock);
                }
                else
                {
                    ModelState.AddModelError("", "لم يتم العثور على تسجيلات الإنتاج أو المخزون");
                    return View(dailySales);
                }

                // Remove the sales record
                _context.DailySales.Remove(dailySales);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DailySalesExists(int id)
        {
            return _context.DailySales.Any(e => e.dailySales_id == id);
        }
    }
}
