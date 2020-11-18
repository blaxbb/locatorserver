using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using LocatorServer.Data;
using LocatorServer.Models;
using Microsoft.AspNetCore.Mvc;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using Microsoft.AspNetCore.Authorization;

namespace LocatorServer.Controllers
{
    /*
     * 
     * 
     * There is a pretty annoying "bug" in aspnet core 5 where using both mvc and webapi will
     * cause the webapi functions to redirect to the login page instead of just returning
     * 401.
     * 
     * Ideally, I would just throw an [Authorize(Roles="authorized")] on the class, but
     * instead I have to manually return Unauthorized for each method.  This is fine, but
     * should not be ignored in the future!!!!!
     * 
     * https://github.com/dotnet/aspnetcore/issues/9039
     * 
     * 
     */

    [Route("api/location")]
    [ApiController]
    public class LocationEntriesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LocationEntriesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class LocationEntryViewModel
        {
            public long ID { get; set; }
            public string Author { get; set; }
            public DateTime Created { get; set; }
            public string Location { get; set; }
            public string SKU { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public LocationEntryViewModel()
            {

            }
            public LocationEntryViewModel(LocationEntry entry)
            {
                ID = entry.ID;
                Author = entry.Author?.RealName ?? "";
                Created = entry.Created;
                Location = entry.Location;
                SKU = entry.SKU;
                X = entry.X;
                Y = entry.Y;
            }

            public LocationEntry Model()
            {
                return new LocationEntry()
                {
                    ID = ID,
                    Created = Created,
                    Location = Location,
                    SKU = SKU,
                    X = X,
                    Y = Y
                };
            }
        }

        // GET: api/LocationEntriesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationEntryViewModel>>> GetLocationEntry()
        {
            if (!User.IsInRole("authorized"))
            {
                return Unauthorized();
            }

            return await _context.LocationEntry.Include(l => l.Author).Select(l => new LocationEntryViewModel(l)).ToListAsync();
        }

        // GET: api/LocationEntriesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationEntryViewModel>> GetLocationEntry(long id)
        {
            if (!User.IsInRole("authorized"))
            {
                return Unauthorized();
            }

            var locationEntry = await _context.LocationEntry.Include(l => l.Author).FirstOrDefaultAsync(l => l.ID == id);

            if (locationEntry == null)
            {
                return NotFound();
            }

            return new LocationEntryViewModel(locationEntry);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<LocationEntryViewModel>>> Search(string location, string sku)
        {
            if (!User.IsInRole("authorized"))
            {
                return Unauthorized();
            }

            return await _context.LocationEntry.Include(l => l.Author).Where(l => l.Location == location && l.SKU == sku).Select(l => new LocationEntryViewModel(l)).ToListAsync();
        }

        //// PUT: api/LocationEntriesApi/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutLocationEntry(long id, LocationEntry locationEntry)
        //{
        //    if (id != locationEntry.ID)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(locationEntry).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!LocationEntryExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/LocationEntriesApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LocationEntryViewModel>> PostLocationEntry(LocationEntryViewModel locationEntryViewModel)
        {
            if (!User.IsInRole("authorized"))
            {
                return Unauthorized();
            }

            locationEntryViewModel.Created = DateTime.Now;
            var model = locationEntryViewModel.Model();
            var user = _context.Users.FirstOrDefault(u => u.Email == HttpContext.User.Identity.Name);
            model.Author = user;
            locationEntryViewModel.Author = model.Author?.RealName ?? "";

            _context.Add(model);
            await _context.SaveChangesAsync();
            locationEntryViewModel.ID = model.ID;
            return CreatedAtAction("GetLocationEntry", new { id = model.ID }, locationEntryViewModel);
        }

        // DELETE: api/LocationEntriesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocationEntry(long id)
        {
            if (!User.IsInRole("authorized"))
            {
                return Unauthorized();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == HttpContext.User.Identity.Name);

            var locationEntry = await _context.LocationEntry.FindAsync(id);
            if (locationEntry == null)
            {
                return NotFound();
            }

            if(locationEntry.Author != user)
            {
                return NotFound();
            }

            _context.LocationEntry.Remove(locationEntry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocationEntryExists(long id)
        {
            return _context.LocationEntry.Any(e => e.ID == id);
        }
    }
}
