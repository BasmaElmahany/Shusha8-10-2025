using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Shusha_project_BackUp.Data;
using Shusha_project_BackUp.Models;

namespace Shusha_project_BackUp.Controllers
{
    [Authorize(Roles = "Admin,Accountant")]
    public class ProfitsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProfitsController (ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
           /* var salesOfWards = _context.DailySales
                                       .Join(
                                                  _context.Wards,
                                                  dailySale => dailySale.wardID,  
                                                  ward => ward.Ward_ID,        
                                                  (dailySale, ward) => new       
                                                  {
                                                      dailySale.wardID,
                                                      ward.WardName,
                                                      dailySale.Total
                                                  }
                                              )
                                          .GroupBy(
                                              ds => new { ds.wardID, ds.WardName } 
                                          )
                                          .Select(group => new
                                          {
                                              WardID = group.Key.wardID,
                                              WardName = group.Key.WardName,
                                              TotalSum = group.Sum(ds => ds.Total)
                                          })
                                          .ToList();*/
           

            var emp_expenses = await _context.Contracts.SumAsync(s => s.Gross_salary);
            var feed_expenses = await _context.feedInventories.SumAsync(s => s.total);
            var feedpuchases_expenses = await _context.FeedPurchases.SumAsync(s => s.total);
            var medicine_expenses = await _context.Medicine_Pruchase.SumAsync(s => s.Total);
            var Electricity_Invoices_expenses = await _context.Electricity_Invoices.SumAsync(s => s.amount);
            var solar_Invoices_expenses = await _context.Solar_Invoices.SumAsync(s => s.amount);

            // Now you can safely add the decimal values
            decimal total = emp_expenses + feed_expenses + feedpuchases_expenses + medicine_expenses + Electricity_Invoices_expenses + solar_Invoices_expenses;
          //  var profit = salesOfWards.Sum(s => s.TotalSum);
            var expensesVm = new WardSalesViewModel
            {
                emp_expenses = (double)emp_expenses,
                feed_expenses = (double)feed_expenses,
                feedpuchases_expenses = (double)feedpuchases_expenses,
                medicine_expenses = (double)medicine_expenses,
                Electricity_Invoices_expenses = (double)Electricity_Invoices_expenses,

                solar_Invoices_expenses = (double)solar_Invoices_expenses,
                Total = (double)total,
               // Sales= salesOfWards.Cast<object>().ToList(),
              //  netProfit = profit

            };


            return View(expensesVm);
        }

        /*public async Task<IActionResult> Expenses ()
        {
            var emp_expenses = await _context.Contracts.SumAsync(s => s.Gross_salary);
            var feed_expenses = await _context.feedInventories.SumAsync(s => s.total);
            var feedpuchases_expenses = await _context.FeedPurchases.SumAsync(s => s.total);
            var medicine_expenses = await _context.Medicine_Pruchase.SumAsync(s => s.Total);
            var Electricity_Invoices_expenses = await _context.Electricity_Invoices.SumAsync(s => s.amount);
            var solar_Invoices_expenses = await _context.Solar_Invoices.SumAsync(s => s.amount);

            // Now you can safely add the decimal values
            decimal total = emp_expenses + feed_expenses + feedpuchases_expenses + medicine_expenses + Electricity_Invoices_expenses + solar_Invoices_expenses;

            var expensesVm = new WardSalesViewModel
            { emp_expenses = (double)emp_expenses,
                feed_expenses = (double)feed_expenses,
                feedpuchases_expenses = (double)feedpuchases_expenses,
                medicine_expenses = (double)medicine_expenses,
                Electricity_Invoices_expenses = (double)Electricity_Invoices_expenses,

                solar_Invoices_expenses = (double)solar_Invoices_expenses,
                Total =(double) total 

            };
            return View (expensesVm);
        }*/
    }
}
