using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS.Areas.Identity.Data;
using POS.Models;

namespace POS.Controllers
{
    public class DtrsController : Controller
    {
        private readonly POSContext _context;

        public DtrsController(POSContext context)
        {
            _context = context;
        }

        // GET: Dtrs
        public async Task<IActionResult> Index()
        {
            var pOSContext = _context.Dtrs.Include(d => d.Employee);
            return View(await pOSContext.ToListAsync());
        }

        // GET: Dtrs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtr = await _context.Dtrs
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(m => m.DtrId == id);
            if (dtr == null)
            {
                return NotFound();
            }

            return View(dtr);
        }

        // GET: Dtrs/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View();
        }

        // POST: Dtrs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DtrId,EmployeeId,TimeIn,TimeOut")] Dtr dtr)
        {

            dtr.DtrDate = DateTime.Now.Date;

            if (ModelState.IsValid)
            {
                _context.Add(dtr);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", dtr.EmployeeId);
            return View(dtr);
        }

        // GET: Dtrs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtr = await _context.Dtrs.FindAsync(id);
            if (dtr == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Email", dtr.EmployeeId);
            return View(dtr);
        }

        // POST: Dtrs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DtrId,EmployeeId,DtrDate,TimeIn,TimeOut")] Dtr dtr)
        {
            if (id != dtr.DtrId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dtr);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DtrExists(dtr.DtrId))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Email", dtr.EmployeeId);
            return View(dtr);
        }

        // GET: Dtrs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dtr = await _context.Dtrs
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(m => m.DtrId == id);
            if (dtr == null)
            {
                return NotFound();
            }

            return View(dtr);
        }

        // POST: Dtrs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dtr = await _context.Dtrs.FindAsync(id);
            if (dtr != null)
            {
                _context.Dtrs.Remove(dtr);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DtrExists(int id)
        {
            return _context.Dtrs.Any(e => e.DtrId == id);
        }
    }
}
