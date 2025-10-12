
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
        public async Task<IActionResult> Index(DateOnly? from, DateOnly? to, int? month)
        {
            var query = _context.TraderSales.AsQueryable();

            // ✅ Apply filters
            if (from.HasValue)
                query = query.Where(s => s.date >= from.Value);
            if (to.HasValue)
                query = query.Where(s => s.date <= to.Value);
            if (month.HasValue && month.Value >= 1 && month.Value <= 12)
                query = query.Where(s => s.date.Month == month.Value);

            var salesList = await query.ToListAsync();


            var filteredSales = await query.ToListAsync();

            // Replace "todaySales" with "filteredSales" everywhere below
            ViewBag.TodayWhite = filteredSales.Sum(s => s.NoOfWhiteEggs);
            ViewBag.TodayNewWhite = filteredSales.Sum(s => s.NoOfNewWhiteEggs);
            ViewBag.TodayMedWhite = filteredSales.Sum(s => s.NoOfMedWhiteEggs);

            ViewBag.TodayBrown = filteredSales.Sum(s => s.NoOfBrownEggs);
            ViewBag.TodayNewBrown = filteredSales.Sum(s => s.NoOfNewBrownEggs);
            ViewBag.TodayMedBrown = filteredSales.Sum(s => s.NoOfMedBrownEggs);

            ViewBag.TodayDouble = filteredSales.Sum(s => s.NoOfDoubleEggs);
            ViewBag.TodayNewDouble = filteredSales.Sum(s => s.NoOfNewDoubleEggs);
            ViewBag.TodayMedDouble = filteredSales.Sum(s => s.NoOfMedDoubleEggs);

            // Total count for filtered range
            ViewBag.TodayTotalEggs =
                ViewBag.TodayWhite + ViewBag.TodayNewWhite + ViewBag.TodayMedWhite +
                ViewBag.TodayBrown + ViewBag.TodayNewBrown + ViewBag.TodayMedBrown +
                ViewBag.TodayDouble + ViewBag.TodayNewDouble + ViewBag.TodayMedDouble;

            // Total sales for filtered range
            ViewBag.TodayTotalSales = filteredSales.Sum(s => s.RequestProceed);


            // ✅ Calculate totals only for filtered data
            var Total_WhEgg = salesList.Sum(s => s.NoOfWhiteEggs);
            var Total_BrEgg = salesList.Sum(s => s.NoOfBrownEggs);
            var Total = Total_WhEgg + Total_BrEgg;
            var total_Wh_sales = salesList.Sum(s => s.NoOfWhiteEggs * s.WhiteEggPrice);
            var total_br_sales = salesList.Sum(s => s.NoOfBrownEggs * s.BrownEggPrice);
            var Total_sales = total_Wh_sales + total_br_sales;

            ViewBag.Total_WhEgg = Total_WhEgg;
            ViewBag.Total_BrEgg = Total_BrEgg;
            ViewBag.Total = Total;
            ViewBag.total_Wh_sales = total_Wh_sales;
            ViewBag.total_br_sales = total_br_sales;
            ViewBag.Total_sales = Total_sales;

            // ✅ Keep filters for the view (so they stay selected)
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");
            ViewBag.Month = month;

            return View(await query.OrderByDescending(s => s.date).ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Id,NoOfWhiteEggs,WhiteEggPrice,NoOfNewWhiteEggs,NewWhiteEggPrice,NoOfMedWhiteEggs,MedWhiteEggPrice,NoOfBrownEggs,BrownEggPrice,NoOfNewBrownEggs,NewBrownEggPrice,NoOfMedBrownEggs,BrownMedEggPrice,NoOfBrokenEggs,BrokenEggPrice,NoOfDoubleEggs,DoubleEggPrice,NoOfNewDoubleEggs,NewDoubleEggPrice,NoOfMedDoubleEggs,MedDoubleEggPrice,IsPaid,date,name_ofTrader")] TraderSales traderSales)
        {
            if (ModelState.IsValid)
            {
                // Calculate Request Proceed
                traderSales.RequestProceed =
                    (traderSales.NoOfWhiteEggs * traderSales.WhiteEggPrice) +
                    (traderSales.NoOfBrownEggs * traderSales.BrownEggPrice) + 
                    (traderSales.BrokenEggPrice * traderSales.NoOfBrokenEggs) +
                   (traderSales.DoubleEggPrice * traderSales.NoOfDoubleEggs) +
                   (traderSales.NoOfNewWhiteEggs * traderSales.NewWhiteEggPrice) +
                   (traderSales.NoOfMedWhiteEggs * traderSales.MedWhiteEggPrice) +
                   (traderSales.NoOfNewBrownEggs * traderSales.NewBrownEggPrice) +
                   (traderSales.NoOfMedBrownEggs * traderSales.BrownMedEggPrice) +
                   (traderSales.NoOfNewDoubleEggs * traderSales.NewDoubleEggPrice) +
                   (traderSales.NoOfMedDoubleEggs * traderSales.MedDoubleEggPrice); ;

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,NoOfWhiteEggs,WhiteEggPrice,NoOfNewWhiteEggs,NewWhiteEggPrice,NoOfMedWhiteEggs,MedWhiteEggPrice,NoOfBrownEggs,BrownEggPrice,NoOfNewBrownEggs,NewBrownEggPrice,NoOfMedBrownEggs,BrownMedEggPrice,NoOfBrokenEggs,BrokenEggPrice,NoOfDoubleEggs,DoubleEggPrice,NoOfNewDoubleEggs,NewDoubleEggPrice,NoOfMedDoubleEggs,MedDoubleEggPrice,IsPaid,date,name_ofTrader")] TraderSales traderSales)
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
                    (traderSales.NoOfBrownEggs * traderSales.BrownEggPrice) +
                    (traderSales.BrokenEggPrice * traderSales.NoOfBrokenEggs) +
                   (traderSales.DoubleEggPrice * traderSales.NoOfDoubleEggs) +
                   (traderSales.NoOfNewWhiteEggs * traderSales.NewWhiteEggPrice) +
                   (traderSales.NoOfMedWhiteEggs * traderSales.MedWhiteEggPrice) +
                   (traderSales.NoOfNewBrownEggs * traderSales.NewBrownEggPrice) +
                   (traderSales.NoOfMedBrownEggs * traderSales.BrownMedEggPrice) +
                   (traderSales.NoOfNewDoubleEggs * traderSales.NewDoubleEggPrice) +
                   (traderSales.NoOfMedDoubleEggs * traderSales.MedDoubleEggPrice); ;

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

        public IActionResult ExportToExcel(DateOnly? from, DateOnly? to, int? month)
        {
            var query = _context.TraderSales.AsQueryable();

            if (from.HasValue) query = query.Where(s => s.date >= from.Value);
            if (to.HasValue) query = query.Where(s => s.date <= to.Value);
            if (month.HasValue && month.Value >= 1 && month.Value <= 12)
                query = query.Where(s => s.date.Month == month.Value);

            var sales = query.OrderBy(s => s.date).ToList();

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Trader Sales");
                ws.View.RightToLeft = true;

                int c = 1;
                // Headers (expanded to include all fields)
                ws.Cells[1, c++].Value = "Id";
                ws.Cells[1, c++].Value = "اسم التاجر";
                ws.Cells[1, c++].Value = "التاريخ";

                ws.Cells[1, c++].Value = "عدد الأبيض (كرتون)";
                ws.Cells[1, c++].Value = "سعر الأبيض";

                ws.Cells[1, c++].Value = "عدد الأبيض (جديد)";
                ws.Cells[1, c++].Value = "سعر الأبيض (جديد)";
                ws.Cells[1, c++].Value = "عدد الأبيض (متوسط)";
                ws.Cells[1, c++].Value = "سعر الأبيض (متوسط)";

                ws.Cells[1, c++].Value = "عدد البني (كرتون)";
                ws.Cells[1, c++].Value = "سعر البني";

                ws.Cells[1, c++].Value = "عدد البني (جديد)";
                ws.Cells[1, c++].Value = "سعر البني (جديد)";
                ws.Cells[1, c++].Value = "عدد البني (متوسط)";
                ws.Cells[1, c++].Value = "سعر البني (متوسط)";

                ws.Cells[1, c++].Value = "عدد المكسور";
                ws.Cells[1, c++].Value = "سعر المكسور";

                ws.Cells[1, c++].Value = "عدد المزدوج";
                ws.Cells[1, c++].Value = "سعر المزدوج";
                ws.Cells[1, c++].Value = "عدد المزدوج (جديد)";
                ws.Cells[1, c++].Value = "سعر المزدوج (جديد)";
                ws.Cells[1, c++].Value = "عدد المزدوج (متوسط)";
                ws.Cells[1, c++].Value = "سعر المزدوج (متوسط)";

                ws.Cells[1, c++].Value = "إجمالي المطلوب";
                ws.Cells[1, c++].Value = "تم السداد؟";

                using (var rng = ws.Cells[1, 1, 1, c - 1])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                int row = 2;
                foreach (var s in sales)
                {
                    int col = 1;
                    ws.Cells[row, col++].Value = s.Id;
                    ws.Cells[row, col++].Value = s.name_ofTrader;
                    ws.Cells[row, col++].Value = s.date.ToString("yyyy-MM-dd");

                    ws.Cells[row, col++].Value = s.NoOfWhiteEggs;
                    ws.Cells[row, col++].Value = s.WhiteEggPrice;

                    ws.Cells[row, col++].Value = s.NoOfNewWhiteEggs;
                    ws.Cells[row, col++].Value = s.NewWhiteEggPrice;
                    ws.Cells[row, col++].Value = s.NoOfMedWhiteEggs;
                    ws.Cells[row, col++].Value = s.MedWhiteEggPrice;

                    ws.Cells[row, col++].Value = s.NoOfBrownEggs;
                    ws.Cells[row, col++].Value = s.BrownEggPrice;

                    ws.Cells[row, col++].Value = s.NoOfNewBrownEggs;
                    ws.Cells[row, col++].Value = s.NewBrownEggPrice;
                    ws.Cells[row, col++].Value = s.NoOfMedBrownEggs;
                    ws.Cells[row, col++].Value = s.BrownMedEggPrice;

                    ws.Cells[row, col++].Value = s.NoOfBrokenEggs;
                    ws.Cells[row, col++].Value = s.BrokenEggPrice;

                    ws.Cells[row, col++].Value = s.NoOfDoubleEggs;
                    ws.Cells[row, col++].Value = s.DoubleEggPrice;
                    ws.Cells[row, col++].Value = s.NoOfNewDoubleEggs;
                    ws.Cells[row, col++].Value = s.NewDoubleEggPrice;
                    ws.Cells[row, col++].Value = s.NoOfMedDoubleEggs;
                    ws.Cells[row, col++].Value = s.MedDoubleEggPrice;

                    ws.Cells[row, col++].Value = s.RequestProceed;
                    ws.Cells[row, col++].Value = s.IsPaid ? "نعم" : "لا";

                    row++;
                }

                // Auto-fit
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                // Optional: freeze header
                ws.View.FreezePanes(2, 1);

                var stream = new MemoryStream(package.GetAsByteArray());
                var fileName = from.HasValue || to.HasValue
                    ? $"مبيعات_المزرعة_للتجار_{from?.ToString("yyyyMMdd")}_الى_{to?.ToString("yyyyMMdd")}.xlsx"
                    : "مبيعات_المزرعة_للتجار.xlsx";

                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
        }




        private bool TraderSalesExists(int id)
        {
            return _context.TraderSales.Any(e => e.Id == id);
        }
    }
}
