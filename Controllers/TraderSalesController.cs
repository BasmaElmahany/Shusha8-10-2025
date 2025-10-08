using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Shusha_project_BackUp;
using Shusha_project_BackUp.Data;

namespace Shusha_project_BackUp.Controllers
{
    public class TraderSalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TraderSalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TraderSales
        public async Task<IActionResult> Index()
        {
           var Total_WhEgg = _context.TraderSales.Sum(s => s.NoOfWhiteEggs);
           var Total_BrEgg = _context.TraderSales.Sum(s => s.NoOfBrownEggs);
           var Total = Total_WhEgg + Total_BrEgg;
           var total_Wh_sales = _context.TraderSales.Sum(s => s.NoOfWhiteEggs * s.WhiteEggPrice);
           var total_br_sales = _context.TraderSales.Sum(s => s.NoOfBrownEggs * s.BrownEggPrice);
           var Total_sales = total_Wh_sales + total_br_sales;
            ViewBag.Total_WhEgg = Total_WhEgg;
            ViewBag.Total_BrEgg = Total_BrEgg;
            ViewBag.Total = Total;
            ViewBag.total_Wh_sales = total_Wh_sales;
            ViewBag.total_br_sales = total_br_sales;
            ViewBag.Total_sales = Total_sales;
            return View(await _context.TraderSales.ToListAsync());
        }

        // GET: TraderSales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var traderSales = await _context.TraderSales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (traderSales == null)
            {
                return NotFound();
            }

            return View(traderSales);
        }

        // GET: TraderSales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TraderSales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NoOfWhiteEggs,WhiteEggPrice,NoOfBrownEggs,BrownEggPrice,NoOfBrokenEggs,BrokenEggPrice,NoOfDoubleEggs,DoubleEggPrice,IsPaid,date,name_ofTrader")] TraderSales traderSales)
        {
            if (ModelState.IsValid)
            {
                // Calculate Request Proceed
                traderSales.RequestProceed =
                    (traderSales.NoOfWhiteEggs * traderSales.WhiteEggPrice) +
                    (traderSales.NoOfBrownEggs * traderSales.BrownEggPrice);

                // Add new trader sale
                _context.Add(traderSales);

                // Get or create Total_Traders row (assuming only one row exists with ID = 1)
                var totalTraders = await _context.Total_Traders.FirstOrDefaultAsync();

                if (totalTraders == null)
                {
                    totalTraders = new Total_Traders
                    {
                        Total_traders = (double)traderSales.RequestProceed
                    };
                    _context.Total_Traders.Add(totalTraders);
                }
                else
                {
                    totalTraders.Total_traders += (double)traderSales.RequestProceed;
                    _context.Total_Traders.Update(totalTraders);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(traderSales);
        }

        // GET: TraderSales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var traderSales = await _context.TraderSales.FindAsync(id);
            if (traderSales == null)
            {
                return NotFound();
            }
            return View(traderSales);
        }

        // POST: TraderSales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NoOfWhiteEggs,WhiteEggPrice,NoOfBrownEggs,BrownEggPrice,NoOfBrokenEggs,BrokenEggPrice,NoOfDoubleEggs,DoubleEggPrice,IsPaid,date,name_ofTrader")] TraderSales traderSales)
        {
            if (id != traderSales.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRecord = await _context.TraderSales.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (existingRecord == null)
                        return NotFound();

                    // Recalculate RequestProceed
                    traderSales.RequestProceed =
                        (traderSales.NoOfWhiteEggs * traderSales.WhiteEggPrice) +
                        (traderSales.NoOfBrownEggs * traderSales.BrownEggPrice);

                    // Update total by removing old and adding new value
                    var totalTraders = await _context.Total_Traders.FirstOrDefaultAsync();
                    if (totalTraders != null)
                    {
                        totalTraders.Total_traders -=(double) existingRecord.RequestProceed;
                        totalTraders.Total_traders += (double)traderSales.RequestProceed;
                        _context.Total_Traders.Update(totalTraders);
                    }

                    _context.Update(traderSales);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TraderSalesExists(traderSales.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(traderSales);
        }


        // GET: TraderSales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var traderSales = await _context.TraderSales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (traderSales == null)
            {
                return NotFound();
            }

            return View(traderSales);
        }

        // POST: TraderSales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var traderSales = await _context.TraderSales.FindAsync(id);
            if (traderSales != null)
            {
                // Update total traders
                var totalTraders = await _context.Total_Traders.FirstOrDefaultAsync();
                if (totalTraders != null)
                {
                    totalTraders.Total_traders -=(double) traderSales.RequestProceed;
                    _context.Total_Traders.Update(totalTraders);
                }

                _context.TraderSales.Remove(traderSales);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        public IActionResult ExportToExcel()
        {
            var sales = _context.TraderSales
         .OrderBy(s => s.date) // Order by date ascending
         .ToList();
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Trader Sales");

                // Set worksheet to Right-To-Left
                worksheet.View.RightToLeft = true;

                // Add column headers
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "اسم التاجر";
                worksheet.Cells[1, 3].Value = "التاريخ";
                worksheet.Cells[1, 4].Value = "عدد البيض الابيض بالكرتونة";
                worksheet.Cells[1, 5].Value = "سعر البيض الابيض";
                worksheet.Cells[1, 6].Value = "عدد البيض البني (بالكرتونة)";
                worksheet.Cells[1, 7].Value = "سعر البيض البني";
                worksheet.Cells[1, 8].Value = "اجمالي المطلوب";
                worksheet.Cells[1, 9].Value = "هل تم التسديد ؟";

                // Format headers (bold & center align)
                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Fill data rows
                int row = 2;
                foreach (var sale in sales)
                {
                    worksheet.Cells[row, 1].Value = sale.Id;
                    worksheet.Cells[row, 2].Value = sale.name_ofTrader;
                    worksheet.Cells[row, 3].Value = sale.date.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 4].Value = sale.NoOfWhiteEggs;
                    worksheet.Cells[row, 5].Value = sale.WhiteEggPrice;
                    worksheet.Cells[row, 6].Value = sale.NoOfBrownEggs;
                    worksheet.Cells[row, 7].Value = sale.BrownEggPrice;
                    worksheet.Cells[row, 8].Value = sale.RequestProceed;
                    worksheet.Cells[row, 9].Value = sale.IsPaid ? "نعم" : "لا"; // Convert boolean to Arabic
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Return the file
                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "مبيعات_المزرعة_للتجار.xlsx");
            }
        }



        private bool TraderSalesExists(int id)
        {
            return _context.TraderSales.Any(e => e.Id == id);
        }
    }
}
