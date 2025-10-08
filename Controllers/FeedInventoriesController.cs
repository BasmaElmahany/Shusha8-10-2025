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
    public class FeedInventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedInventoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FeedInventories
        public async Task<IActionResult> Index()
        {
            return View(await _context.feedInventories.ToListAsync());
        }

        // GET: FeedInventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedInventory = await _context.feedInventories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedInventory == null)
            {
                return NotFound();
            }

            return View(feedInventory);
        }

        // GET: FeedInventories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FeedInventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tons_qty,ton_cost")] FeedInventory feedInventory)
        {
            feedInventory.Date = DateTime.Now;
            feedInventory.total = feedInventory.ton_cost * feedInventory.Tons_qty;
            if (ModelState.IsValid)
            {
                _context.Add(feedInventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feedInventory);
        }

        // GET: FeedInventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedInventory = await _context.feedInventories.FindAsync(id);
            if (feedInventory == null)
            {
                return NotFound();
            }
            return View(feedInventory);
        }

        // POST: FeedInventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tons_qty,ton_cost,Date,total")] FeedInventory feedInventory)
        {
            if (id != feedInventory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedInventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedInventoryExists(feedInventory.Id))
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
            return View(feedInventory);
        }

        // GET: FeedInventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedInventory = await _context.feedInventories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedInventory == null)
            {
                return NotFound();
            }

            return View(feedInventory);
        }

        // POST: FeedInventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedInventory = await _context.feedInventories.FindAsync(id);
            if (feedInventory != null)
            {
                _context.feedInventories.Remove(feedInventory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedInventoryExists(int id)
        {
            return _context.feedInventories.Any(e => e.Id == id);
        }
    }
}
