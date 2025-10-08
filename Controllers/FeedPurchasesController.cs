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
    [Authorize(Roles = "Admin,Accountant")]
    public class FeedPurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedPurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FeedPurchases
        public async Task<IActionResult> Index()
        {
            return View(await _context.FeedPurchases.ToListAsync());
        }

        // GET: FeedPurchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedPurchases = await _context.FeedPurchases
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedPurchases == null)
            {
                return NotFound();
            }

            return View(feedPurchases);
        }

        // GET: FeedPurchases/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FeedPurchases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tons_qty,price")] FeedPurchases feedPurchases)
        {
            feedPurchases.Date = DateTime.Now;
            feedPurchases.total = feedPurchases.price * feedPurchases.Tons_qty;
            if (ModelState.IsValid)
            {
                _context.Add(feedPurchases);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feedPurchases);
        }

        // GET: FeedPurchases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedPurchases = await _context.FeedPurchases.FindAsync(id);
            if (feedPurchases == null)
            {
                return NotFound();
            }
            return View(feedPurchases);
        }

        // POST: FeedPurchases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tons_qty,price,Date,total")] FeedPurchases feedPurchases)
        {
            if (id != feedPurchases.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedPurchases);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedPurchasesExists(feedPurchases.Id))
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
            return View(feedPurchases);
        }

        // GET: FeedPurchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedPurchases = await _context.FeedPurchases
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedPurchases == null)
            {
                return NotFound();
            }

            return View(feedPurchases);
        }

        // POST: FeedPurchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedPurchases = await _context.FeedPurchases.FindAsync(id);
            if (feedPurchases != null)
            {
                _context.FeedPurchases.Remove(feedPurchases);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedPurchasesExists(int id)
        {
            return _context.FeedPurchases.Any(e => e.Id == id);
        }
    }
}
