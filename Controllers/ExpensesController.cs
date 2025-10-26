using Microsoft.AspNetCore.Mvc;
using Shusha_project_BackUp.Data;
using Shusha_project_BackUp.Models;

namespace Shusha_project_BackUp.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Show list of all expenses
        public IActionResult Index()
        {
            var expenses = _context.Expenses.OrderByDescending(e => e.date).ToList();
            var totals = _context.Expenses_total.OrderByDescending(t => t.year).ToList();

            ViewBag.Totals = totals;
            return View(expenses);
        }

        // ✅ Show form to add multiple expenses at once
        public IActionResult AddAll()
        {
            var model = new ExpensesFormViewModel
            {
                Date = DateOnly.FromDateTime(DateTime.Now)
            };
            return View(model);
        }

        // ✅ Add all expenses (one per type)
        [HttpPost]
        public async Task<IActionResult> AddAll(ExpensesFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var date = model.Date;

            // ✅ Determine fiscal year dynamically
            int fiscalYear = GetFiscalYear(date);

            // ✅ Prepare expenses list
            var expenses = new List<Expenses>
    {
        new Expenses { type1 = Expenses_type.salaries, amount1 = model.Salaries, date = date },
        new Expenses { type1 = Expenses_type.feed, amount1 = model.Feed, date = date },
        new Expenses { type1 = Expenses_type.herd, amount1 = model.Herd, date = date },
        new Expenses { type1 = Expenses_type.medicine, amount1 = model.Medicine, date = date },
        new Expenses { type1 = Expenses_type.cartoon_plates, amount1 = model.Cartoon_Plates, date = date },
        new Expenses { type1 = Expenses_type.solar, amount1 = model.Solar, date = date },
        new Expenses { type1 = Expenses_type.licence, amount1 = model.Licence, date = date },
        new Expenses { type1 = Expenses_type.sample_analysis, amount1 = model.Sample_Analysis, date = date },
        new Expenses { type1 = Expenses_type.prints, amount1 = model.Prints, date = date },
        new Expenses { type1 = Expenses_type.adv, amount1 = model.Adv, date = date },
        new Expenses { type1 = Expenses_type.maintainance, amount1 = model.Maintainance, date = date },
        new Expenses { type1 = Expenses_type.electricity, amount1 = model.Electricity, date = date },
    };

            // ✅ Ignore empty values
            expenses = expenses.Where(e => e.amount1 > 0).ToList();

            _context.Expenses.AddRange(expenses);

            // ✅ Calculate total for this fiscal year
            decimal yearTotal = expenses.Sum(e => e.amount1);

            // ✅ Update or create fiscal total record
            var totalRecord = _context.Expenses_total.FirstOrDefault(t => t.year == fiscalYear);
            if (totalRecord == null)
            {
                totalRecord = new Expenses_total
                {
                    year = fiscalYear,
                    Total = yearTotal
                };
                _context.Expenses_total.Add(totalRecord);
            }
            else
            {
                totalRecord.Total += yearTotal;
                _context.Expenses_total.Update(totalRecord);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"✅ تم حفظ المصروفات للسنة المالية {fiscalYear}/{fiscalYear + 1} بنجاح.";
            return RedirectToAction("Index");
        }

        // ✅ Helper method to detect fiscal year automatically
        private int GetFiscalYear(DateOnly date)
        {
            // السنة المالية تبدأ من 1 يوليو وتنتهي في 30 يونيو
            if (date.Month >= 7)
            {
                // من 1/7 إلى 31/12 → السنة الحالية
                return date.Year;
            }
            else
            {
                // من 1/1 إلى 30/6 → تعتبر ضمن السنة السابقة مالياً
                return date.Year - 1;
            }
        }


        // ✅ Edit total record
        public IActionResult EditTotal(int id)
        {
            var total = _context.Expenses_total.Find(id);
            if (total == null)
                return NotFound();

            return View(total);
        }

        [HttpPost]
        public async Task<IActionResult> EditTotal(Expenses_total model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Expenses_total.Update(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم تعديل إجمالي المصروفات بنجاح ✅";
            return RedirectToAction("Index");
        }

        // ✅ Delete total record
        /*public async Task<IActionResult> DeleteTotal(int id)
        {
            var total = await _context.Expenses_total.FindAsync(id);
            if (total == null)
                return NotFound();

            _context.Expenses_total.Remove(total);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حذف إجمالي السنة المالية بنجاح 🗑️";
            return RedirectToAction("Index");
        }*/

        // ✅ Helper: Determine fiscal year
       
    }
}
