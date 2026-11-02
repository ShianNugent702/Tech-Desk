using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tech_Desk.Data;
using Tech_Desk.Models;

namespace Tech_Desk.Controllers
{
    public class DevicesController : Controller
    {
        private readonly AppDbContext _context;

        public DevicesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /devices
        public async Task<IActionResult> Index(string searchString, bool availableOnly = false)
        {
            // 1. Start with a query (this doesn't hit the database yet)
            var query = _context.Devices.AsQueryable();

            // 2. Filter by name (if the user typed something)
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(d => d.Name.Contains(searchString));
            }

            // 3. Filter by "Available only" (if the box is ticked)
            if (availableOnly)
            {
                query = query.Where(d => !d.IsOnLoan);
            }

            // 4. Always sort by AssetTag A to Z
            query = query.OrderBy(d => d.AssetTag);

            // 5. Execute the query against the database
            var devices = await query.ToListAsync();

            // 6. Pass the count and the search values to the view
            ViewBag.DeviceCount = devices.Count;
            ViewBag.SearchString = searchString;
            ViewBag.AvailableOnly = availableOnly;

            return View(devices);
        }

        // GET: /devices/details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var device = await _context.Devices.FirstOrDefaultAsync(d => d.Id == id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }
        // GET: /devices/add
        public IActionResult Add()
        {
            return View(new Device());
        }

        // POST: /devices/add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Device device)
        {
            // Check for duplicate asset tag (since we have a unique index in the database)
            bool tagTaken = await _context.Devices.AnyAsync(d => d.AssetTag == device.AssetTag);
            if (tagTaken)
            {
                ModelState.AddModelError("AssetTag", "Another device already has that asset tag.");
            }

            if (ModelState.IsValid)
            {
                _context.Devices.Add(device);
                await _context.SaveChangesAsync();

                // Show a one-time success message
                TempData["Message"] = $"{device.AssetTag} {device.Name} was added.";
                return RedirectToAction(nameof(Index));
            }

            return View(device); // If there are errors, show the form again with what they typed
        }
        // GET: /devices/edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null) return NotFound(); // 404 if the id doesn't exist
            return View(device);
        }

        // POST: /devices/edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Device device)
        {
            if (id != device.Id) return NotFound();

            // Check if another device already has this AssetTag
            bool tagTaken = await _context.Devices.AnyAsync(d => d.AssetTag == device.AssetTag && d.Id != device.Id);

            if (tagTaken)
            {
                // This puts the error message right next to the Asset Tag box
                ModelState.AddModelError("AssetTag", "Another device already has that asset tag.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Devices.Any(e => e.Id == device.Id)) return NotFound();
                    throw;
                }

                TempData["Message"] = $"{device.AssetTag} {device.Name} was updated.";
                return RedirectToAction(nameof(Index));
            }

            return View(device); // Show form again with typed values and errors
        }
        // GET: /devices/delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var device = await _context.Devices.FirstOrDefaultAsync(d => d.Id == id);
            if (device == null)
            {
                TempData["Message"] = "That device was already deleted.";
                return RedirectToAction(nameof(Index));
            }
            return View(device);
        }

        // POST: /devices/delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                // If someone opened two tabs and clicked delete in both
                TempData["Message"] = "That device was already deleted.";
                return RedirectToAction(nameof(Index));
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{device.AssetTag} {device.Name} was deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}