using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LocatorServer.Data;
using LocatorServer.Models;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace LocatorServer.Controllers
{
    [Authorize(Roles = "admin")]
    public class LocationEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LocationEntries
        public async Task<IActionResult> Index(string sort, string locationFilter, string skuFilter, string authorFilter, int? page)
        {
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentLocationFilter = locationFilter;
            ViewBag.CurrentSkuFilter = skuFilter;
            ViewBag.CurrentAuthorFilter = authorFilter;

            var result = _context.LocationEntry.Include(l => l.Author).Select(l => l);

            if (!string.IsNullOrWhiteSpace(locationFilter))
            {
                result = result.Where(l => l.Location == locationFilter);
            }

            if (!string.IsNullOrWhiteSpace(skuFilter))
            {
                result = result.Where(l => l.SKU == skuFilter);
            }

            if (!string.IsNullOrWhiteSpace(authorFilter))
            {
                result = result.Where(l => l.Author.UserName.Contains(authorFilter));
            }

            switch (sort)
            {
                case "created_asc":
                    ViewBag.SortParam = "created_desc";
                    result = result.OrderBy(l => l.Created);
                    break;
                default:
                case "created_desc":
                    result = result.OrderByDescending(l => l.Created);
                    ViewBag.SortParam = "created_asc";
                    break;
            }

            int pageSize = 50;
            int pageNumber = page ?? 1;

            return View(result.ToPagedList(pageNumber, pageSize));
        }

        // GET: LocationEntries/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationEntry = await _context.LocationEntry.Include(l => l.Author)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (locationEntry == null)
            {
                return NotFound();
            }

            return View(locationEntry);
        }

        // GET: LocationEntries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LocationEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Location,SKU,X,Y")] LocationEntry locationEntry)
        {
            locationEntry.Created = DateTime.Now;
            var user = _context.Users.FirstOrDefault(u => u.Email == HttpContext.User.Identity.Name);
            locationEntry.Author = user;
            if (ModelState.IsValid)
            {
                _context.Add(locationEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(locationEntry);
        }

        // GET: LocationEntries/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationEntry = await _context.LocationEntry.FindAsync(id);
            if (locationEntry == null)
            {
                return NotFound();
            }
            return View(locationEntry);
        }

        // POST: LocationEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ID,Location,SKU,X,Y")] LocationEntry locationEntry)
        {
            if (id != locationEntry.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locationEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationEntryExists(locationEntry.ID))
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
            return View(locationEntry);
        }

        // GET: LocationEntries/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationEntry = await _context.LocationEntry
                .FirstOrDefaultAsync(m => m.ID == id);
            if (locationEntry == null)
            {
                return NotFound();
            }

            return View(locationEntry);
        }

        // POST: LocationEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var locationEntry = await _context.LocationEntry.FindAsync(id);
            _context.LocationEntry.Remove(locationEntry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationEntryExists(long id)
        {
            return _context.LocationEntry.Any(e => e.ID == id);
        }
    }
}
