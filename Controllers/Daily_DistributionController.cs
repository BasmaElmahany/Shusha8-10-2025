using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Shusha_project_BackUp;
using Shusha_project_BackUp.Data;


namespace Shusha_project_BackUp.Controllers
{
   
    public class Daily_DistributionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Daily_DistributionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Accountant,proceeds")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Daily_Distributions.Include(d => d.Center).OrderByDescending(d => d.Date);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin,Accountant,proceeds")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daily_Distribution = await _context.Daily_Distributions
                .Include(d => d.Center)
                .FirstOrDefaultAsync(m => m.dis_id == id);
            if (daily_Distribution == null)
            {
                return NotFound();
            }

            return View(daily_Distribution);
        }

        [Authorize(Roles = "Admin,Accountant")]
        public IActionResult Create()
        {
            ViewBag.centerId= new SelectList(_context.Centers, "centerId", "centerName");
            return View();
        }

        // POST: Daily_Distribution/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Daily_Distribution/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> Create([Bind("dis_id,centerId,WhEgg_platesnumber,doubleEgg_platesnumber,brEgg_platesnumber,BrokenEgg_platesnumber,Date")] Daily_Distribution daily_Distribution)
        {
            if (ModelState.IsValid)
            {
                // Check if a record already exists for the same center and date
                var existingDistribution = await _context.Daily_Distributions
                    .FirstOrDefaultAsync(d => d.centerId == daily_Distribution.centerId && d.Date == daily_Distribution.Date);

                if (existingDistribution != null)
                {
                    ModelState.AddModelError(string.Empty, "تم تسجيل توزيع لهذا المركز في هذا اليوم بالفعل.");
                    ViewBag.centerId = new SelectList(_context.Centers, "centerId", "centerName", daily_Distribution.centerId);
                    return View(daily_Distribution);
                }

                // Get the current egg stock
                var eggStock = await _context.Egg_stock.FirstOrDefaultAsync(es => es.Id == 3); // Assuming you only have one egg stock
                if (eggStock == null)
                {
                    ModelState.AddModelError(string.Empty, "لا يوجد مخزون بيض");
                    return View(daily_Distribution);
                }

                // Subtract the distribution quantities from the stock
                eggStock.plates_whiteEggs -= daily_Distribution.WhEgg_platesnumber;
                eggStock.plates_BrownEggs -= daily_Distribution.brEgg_platesnumber;
                eggStock.doubleEggs -= daily_Distribution.doubleEgg_platesnumber;
                eggStock.brokenPlates -= daily_Distribution.BrokenEgg_platesnumber;

                // Recalculate the total after subtraction
                eggStock.RecalculateTotal();

                // Try to get the price for today
                var todayPrice = await _context.Prices
                    .Where(p => p.Date == DateOnly.FromDateTime(DateTime.Now))
                    .FirstOrDefaultAsync();

                // If no price for today, get the latest price
                if (todayPrice == null)
                {
                    todayPrice = await _context.Prices
                        .OrderByDescending(p => p.Date)
                        .FirstOrDefaultAsync();
                }

                if (todayPrice == null)
                {
                    ModelState.AddModelError(string.Empty, "لا توجد أسعار متاحة.");
                    return View(daily_Distribution);
                }

                // Calculate the requested proceeds amount
                decimal requestedAmount = daily_Distribution.BrokenEgg_platesnumber * todayPrice.broken_Egg_Price
                                          + daily_Distribution.doubleEgg_platesnumber * todayPrice.double_Egg_Price
                                          + daily_Distribution.WhEgg_platesnumber * todayPrice.white_Egg_Price
                                          + daily_Distribution.brEgg_platesnumber * todayPrice.brown_Egg_Price;

                // Check if there is an existing request for this center
                var existingRequest = await _context.Request_Proceeds
                    .FirstOrDefaultAsync(r => r.centerID == daily_Distribution.centerId);

                if (existingRequest != null)
                {
                    // Add the new amount to the existing requested proceeds
                    existingRequest.requested_proceeds += requestedAmount;
                    _context.Update(existingRequest);
                }
                else
                {
                    // Create a new Request_Proceeds entry
                    var newRequest = new Request_Proceeds
                    {
                        centerID = daily_Distribution.centerId,
                        requested_proceeds = requestedAmount
                    };
                    _context.Add(newRequest);
                }

                // Save the updated stock back to the database
                _context.Update(eggStock);

                // Add the daily distribution record to the database
                _context.Add(daily_Distribution);
                await _context.SaveChangesAsync();

                // Redirect to the Index view
                return RedirectToAction(nameof(Index));
            }

            // If model state is not valid, return the view with errors
            ViewBag.centerId = new SelectList(_context.Centers, "centerId", "centerName", daily_Distribution.centerId);
            return View(daily_Distribution);
        }


        // GET: Daily_Distribution/Edit/5
        // GET: Daily_Distribution/Edit/5
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daily_Distribution = await _context.Daily_Distributions.FindAsync(id);
            if (daily_Distribution == null)
            {
                return NotFound();
            }

            ViewBag.centerId = new SelectList(_context.Centers, "centerId", "centerName", daily_Distribution.centerId);
            return View(daily_Distribution);
        }

        // POST: Daily_Distribution/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> Edit(int id, [Bind("dis_id,centerId,WhEgg_platesnumber,doubleEgg_platesnumber,brEgg_platesnumber,BrokenEgg_platesnumber")] Daily_Distribution daily_Distribution)
        {
            if (id != daily_Distribution.dis_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                
                    // Get today's price or the latest available price
                    var todayPrice = await _context.Prices
                        .Where(p => p.Date == DateOnly.FromDateTime(DateTime.Now))
                        .FirstOrDefaultAsync();

                    if (todayPrice == null)
                    {
                        todayPrice = await _context.Prices
                            .OrderByDescending(p => p.Date)
                            .FirstOrDefaultAsync();
                    }

                    if (todayPrice == null)
                    {
                        ModelState.AddModelError(string.Empty, "لا يوجد اسعار");
                        return View(daily_Distribution);
                    } 



                    // Calculate the requested proceeds amount
                    decimal requestedAmount = daily_Distribution.BrokenEgg_platesnumber * todayPrice.broken_Egg_Price
                                              + daily_Distribution.doubleEgg_platesnumber * todayPrice.double_Egg_Price
                                              + daily_Distribution.WhEgg_platesnumber * todayPrice.white_Egg_Price
                                              + daily_Distribution.brEgg_platesnumber * todayPrice.brown_Egg_Price;

                    // Check if there is an existing request for this center
                    var existingRequest = await _context.Request_Proceeds
                        .FirstOrDefaultAsync(r => r.centerID == daily_Distribution.centerId);

                    if (existingRequest != null)
                    {
                        // Add the new amount to the existing requested proceeds
                        existingRequest.requested_proceeds += requestedAmount;
                        _context.Update(existingRequest);
                    }
                    else
                    {
                        // Create a new Request_Proceeds entry
                        var newRequest = new Request_Proceeds
                        {
                            centerID = daily_Distribution.centerId,
                            requested_proceeds = requestedAmount
                        };
                        _context.Add(newRequest);
                    }

                 
                    _context.Update(daily_Distribution);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Daily_DistributionExists(daily_Distribution.dis_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirect to the Index view
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, return the view with errors
            ViewBag.centerId = new SelectList(_context.Centers, "centerId", "centerName", daily_Distribution.centerId);
            return View(daily_Distribution);
        }

        // POST: Daily_Distribution/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find the Daily_Distribution record by its ID
            var daily_Distribution = await _context.Daily_Distributions.FindAsync(id);

            if (daily_Distribution != null)
            {
                // Get the current egg stock
                var eggStock = await _context.Egg_stock.FirstOrDefaultAsync(es => es.Id == 1); // Assuming there's one egg stock entry

                if (eggStock != null)
                {
                    // Restore the stock by adding back the quantities
                    eggStock.plates_whiteEggs += daily_Distribution.WhEgg_platesnumber;
                    eggStock.plates_BrownEggs += daily_Distribution.brEgg_platesnumber;
                    eggStock.doubleEggs += daily_Distribution.doubleEgg_platesnumber;
                    eggStock.brokenPlates += daily_Distribution.BrokenEgg_platesnumber;

                    // Recalculate the total stock
                    eggStock.RecalculateTotal();

                    // Update the egg stock in the database
                    _context.Update(eggStock);
                }

                // Get today's price or the latest available price
                var todayPrice = await _context.Prices
                    .Where(p => p.Date == DateOnly.FromDateTime(DateTime.Now))
                    .FirstOrDefaultAsync();

                if (todayPrice == null)
                {
                    todayPrice = await _context.Prices
                        .OrderByDescending(p => p.Date)
                        .FirstOrDefaultAsync();
                }

                // Calculate the proceeds that were requested for this record
                if (todayPrice != null)
                {
                    decimal requestedAmount = daily_Distribution.BrokenEgg_platesnumber * todayPrice.broken_Egg_Price
                                              + daily_Distribution.doubleEgg_platesnumber * todayPrice.double_Egg_Price
                                              + daily_Distribution.WhEgg_platesnumber * todayPrice.white_Egg_Price
                                              + daily_Distribution.brEgg_platesnumber * todayPrice.brown_Egg_Price;

                    // Adjust the requested proceeds for the center
                    var existingRequest = await _context.Request_Proceeds
                        .FirstOrDefaultAsync(r => r.centerID == daily_Distribution.centerId);

                    if (existingRequest != null)
                    {
                        // Subtract the deleted record's amount from the existing requested proceeds
                        existingRequest.requested_proceeds -= requestedAmount;

                        // If the proceeds become zero or less, consider deleting the request record
                        if (existingRequest.requested_proceeds <= 0)
                        {
                            _context.Request_Proceeds.Remove(existingRequest);
                        }
                        else
                        {
                            _context.Update(existingRequest);
                        }
                    }
                }

                // Remove the Daily_Distribution record
                _context.Daily_Distributions.Remove(daily_Distribution);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the Index view
            return RedirectToAction(nameof(Index));
        }


        private bool Daily_DistributionExists(int id)
        {
            return _context.Daily_Distributions.Any(e => e.dis_id == id);
        }
        [Authorize(Roles = "Admin,Accountant")]
        public IActionResult ExportTodayToExcel()
        {
            DateTime today = DateTime.Today;

            var distributions = _context.Daily_Distributions
                .Include(d => d.Center)
                .Where(d => d.Date.Date == today)
                .ToList();

            if (!distributions.Any())
            {
                TempData["ErrorMessage"] = "لا يوجد بيانات لهذا اليوم.";
                return RedirectToAction("Index");
            }

            // Fetch the latest prices for today
            var priceForDay = _context.Prices
                .Where(p => p.Date <= DateOnly.FromDateTime(today))
                .OrderByDescending(p => p.Date)
                .FirstOrDefault();

            if (priceForDay == null)
            {
                TempData["ErrorMessage"] = "لم يتم العثور على أسعار لهذا اليوم.";
                return RedirectToAction("Index");
            }

            decimal whiteEggPrice = priceForDay.white_Egg_Price;
            decimal brownEggPrice = priceForDay.brown_Egg_Price;
            decimal doubleEggPrice = priceForDay.double_Egg_Price;
            decimal brokenEggPrice = priceForDay.broken_Egg_Price;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("التوزيع اليومي");
                worksheet.View.RightToLeft = true;

                // Set Header Row
                string[] headers = {
            "المركز", "التاريخ",
            "بيض أبيض (أطباق)", "سعر الطبق", "الإيراد (بيض أبيض)",
            "بيض بني (أطباق)", "سعر الطبق", "الإيراد (بيض بني)",
            "بيض مزدوج (أطباق)", "سعر الطبق", "الإيراد (بيض مزدوج)",
            "بيض مكسور (أطباق)", "سعر الطبق", "الإيراد (بيض مكسور)",
            "الإجمالي", "إجمالي الإيرادات"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                int row = 2;
                int totalWhiteEggs = 0, totalBrownEggs = 0, totalDoubleEggs = 0, totalBrokenEggs = 0, grandTotal = 0;
                decimal totalWhiteRevenue = 0, totalBrownRevenue = 0, totalDoubleRevenue = 0, totalBrokenRevenue = 0, totalRevenue = 0;

                foreach (var item in distributions)
                {
                    decimal whiteRevenue = item.WhEgg_platesnumber * whiteEggPrice;
                    decimal brownRevenue = item.brEgg_platesnumber * brownEggPrice;
                    decimal doubleRevenue = item.doubleEgg_platesnumber * doubleEggPrice;
                    decimal brokenRevenue = item.BrokenEgg_platesnumber * brokenEggPrice;
                    decimal dailyTotalRevenue = whiteRevenue + brownRevenue + doubleRevenue + brokenRevenue;

                    worksheet.Cells[row, 1].Value = item.Center?.centerName ?? "غير محدد";
                    worksheet.Cells[row, 2].Value = item.Date.ToString("yyyy-MM-dd", new CultureInfo("ar-EG"));

                    worksheet.Cells[row, 3].Value = item.WhEgg_platesnumber;
                    worksheet.Cells[row, 4].Value = whiteEggPrice;
                    worksheet.Cells[row, 5].Value = whiteRevenue;

                    worksheet.Cells[row, 6].Value = item.brEgg_platesnumber;
                    worksheet.Cells[row, 7].Value = brownEggPrice;
                    worksheet.Cells[row, 8].Value = brownRevenue;

                    worksheet.Cells[row, 9].Value = item.doubleEgg_platesnumber;
                    worksheet.Cells[row, 10].Value = doubleEggPrice;
                    worksheet.Cells[row, 11].Value = doubleRevenue;

                    worksheet.Cells[row, 12].Value = item.BrokenEgg_platesnumber;
                    worksheet.Cells[row, 13].Value = brokenEggPrice;
                    worksheet.Cells[row, 14].Value = brokenRevenue;

                    worksheet.Cells[row, 15].Value = item.total;
                    worksheet.Cells[row, 16].Value = dailyTotalRevenue;

                    // Accumulate totals
                    totalWhiteEggs += item.WhEgg_platesnumber;
                    totalBrownEggs += item.brEgg_platesnumber;
                    totalDoubleEggs += (int)item.doubleEgg_platesnumber;
                    totalBrokenEggs += (int)item.BrokenEgg_platesnumber;
                    grandTotal += item.total;

                    totalWhiteRevenue += whiteRevenue;
                    totalBrownRevenue += brownRevenue;
                    totalDoubleRevenue += doubleRevenue;
                    totalBrokenRevenue += brokenRevenue;
                    totalRevenue += dailyTotalRevenue;

                    row++;
                }

                // Add the totals row
                worksheet.Cells[row, 1].Value = "الإجمالي لكل المراكز";
                worksheet.Cells[row, 1, row, 2].Merge = true;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[row, 3].Value = totalWhiteEggs;
                worksheet.Cells[row, 5].Value = totalWhiteRevenue;

                worksheet.Cells[row, 6].Value = totalBrownEggs;
                worksheet.Cells[row, 8].Value = totalBrownRevenue;

                worksheet.Cells[row, 9].Value = totalDoubleEggs;
                worksheet.Cells[row, 11].Value = totalDoubleRevenue;

                worksheet.Cells[row, 12].Value = totalBrokenEggs;
                worksheet.Cells[row, 14].Value = totalBrokenRevenue;

                worksheet.Cells[row, 15].Value = grandTotal;
                worksheet.Cells[row, 16].Value = totalRevenue;

                // Style the totals row
                using (var totalRow = worksheet.Cells[row, 1, row, 16])
                {
                    totalRow.Style.Font.Bold = true;
                    totalRow.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    totalRow.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    totalRow.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Daily_Distribution_{today:yyyy-MM-dd}.xlsx");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> ExportToExcel(DateTime startDate, DateTime endDate)
        {
            var distributions = await _context.Daily_Distributions
                .Include(d => d.Center)
                .Where(d => d.Date >= startDate && d.Date <= endDate)
                .ToListAsync();

            if (!distributions.Any())
            {
                return Content("لا توجد بيانات في الفترة المحددة.");
            }

            // Fetch all prices within the date range
            var priceList = await _context.Prices
                .Where(p => p.Date >= DateOnly.FromDateTime(startDate) && p.Date <= DateOnly.FromDateTime(endDate))
                .ToDictionaryAsync(p => p.Date, p => p);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("التوزيع اليومي");
                worksheet.View.RightToLeft = true;

                // Define Headers
                var headers = new string[]
                {
            "التاريخ", "المركز",
            "بيض أبيض (أطباق)", "سعر الطبق", "الإيراد (بيض أبيض)",
            "بيض بني (أطباق)", "سعر الطبق", "الإيراد (بيض بني)",
            "بيض مزدوج (أطباق)", "سعر الطبق", "الإيراد (بيض مزدوج)",
            "بيض مكسور (أطباق)", "سعر الطبق", "الإيراد (بيض مكسور)",
            "الإجمالي", "إجمالي الإيرادات"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                int row = 2;
                int totalWhiteEggs = 0, totalBrownEggs = 0, totalDoubleEggs = 0, totalBrokenEggs = 0, grandTotal = 0;
                decimal totalWhiteRevenue = 0, totalBrownRevenue = 0, totalDoubleRevenue = 0, totalBrokenRevenue = 0, totalRevenue = 0;

                // Populate Data
                foreach (var dist in distributions)
                {
                    // Fetch price for the specific date, fallback to latest price if not found
                    if (!priceList.TryGetValue(DateOnly.FromDateTime(dist.Date), out var priceForDay))
                    {
                        priceForDay = await _context.Prices.OrderByDescending(p => p.Date).FirstOrDefaultAsync();
                    }

                    if (priceForDay == null)
                    {
                        continue; // Skip record if no price is available
                    }

                    decimal whiteEggPrice = priceForDay.white_Egg_Price;
                    decimal brownEggPrice = priceForDay.brown_Egg_Price;
                    decimal doubleEggPrice = priceForDay.double_Egg_Price;
                    decimal brokenEggPrice = priceForDay.broken_Egg_Price;

                    decimal whiteRevenue = dist.WhEgg_platesnumber * whiteEggPrice;
                    decimal brownRevenue = dist.brEgg_platesnumber * brownEggPrice;
                    decimal doubleRevenue = dist.doubleEgg_platesnumber * doubleEggPrice;
                    decimal brokenRevenue = dist.BrokenEgg_platesnumber * brokenEggPrice;
                    decimal dailyTotalRevenue = whiteRevenue + brownRevenue + doubleRevenue + brokenRevenue;

                    worksheet.Cells[row, 1].Value = dist.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 2].Value = dist.Center.centerName;

                    worksheet.Cells[row, 3].Value = dist.WhEgg_platesnumber;
                    worksheet.Cells[row, 4].Value = whiteEggPrice;
                    worksheet.Cells[row, 5].Value = whiteRevenue;

                    worksheet.Cells[row, 6].Value = dist.brEgg_platesnumber;
                    worksheet.Cells[row, 7].Value = brownEggPrice;
                    worksheet.Cells[row, 8].Value = brownRevenue;

                    worksheet.Cells[row, 9].Value = dist.doubleEgg_platesnumber;
                    worksheet.Cells[row, 10].Value = doubleEggPrice;
                    worksheet.Cells[row, 11].Value = doubleRevenue;

                    worksheet.Cells[row, 12].Value = dist.BrokenEgg_platesnumber;
                    worksheet.Cells[row, 13].Value = brokenEggPrice;
                    worksheet.Cells[row, 14].Value = brokenRevenue;

                    worksheet.Cells[row, 15].Value = dist.total;
                    worksheet.Cells[row, 16].Value = dailyTotalRevenue;

                    // Accumulate totals
                    totalWhiteEggs += dist.WhEgg_platesnumber;
                    totalBrownEggs += dist.brEgg_platesnumber;
                    totalDoubleEggs += (int)dist.doubleEgg_platesnumber;
                    totalBrokenEggs += (int)dist.BrokenEgg_platesnumber;
                    grandTotal += dist.total;

                    totalWhiteRevenue += whiteRevenue;
                    totalBrownRevenue += brownRevenue;
                    totalDoubleRevenue += doubleRevenue;
                    totalBrokenRevenue += brokenRevenue;
                    totalRevenue += dailyTotalRevenue;

                    row++;
                }

                // Add Totals Row
                worksheet.Cells[row, 1].Value = "الإجمالي لكل المراكز";
                worksheet.Cells[row, 1, row, 2].Merge = true;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[row, 3].Value = totalWhiteEggs;
                worksheet.Cells[row, 5].Value = totalWhiteRevenue;

                worksheet.Cells[row, 6].Value = totalBrownEggs;
                worksheet.Cells[row, 8].Value = totalBrownRevenue;

                worksheet.Cells[row, 9].Value = totalDoubleEggs;
                worksheet.Cells[row, 11].Value = totalDoubleRevenue;

                worksheet.Cells[row, 12].Value = totalBrokenEggs;
                worksheet.Cells[row, 14].Value = totalBrokenRevenue;

                worksheet.Cells[row, 15].Value = grandTotal;
                worksheet.Cells[row, 16].Value = totalRevenue;

                // Style the Totals Row
                using (var totalRow = worksheet.Cells[row, 1, row, 16])
                {
                    totalRow.Style.Font.Bold = true;
                    totalRow.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    totalRow.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    totalRow.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                // Generate file
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string fileName = $"التوزيع_اليومي_{startDate:yyyyMMdd}_إلى_{endDate:yyyyMMdd}.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }



    }
}
