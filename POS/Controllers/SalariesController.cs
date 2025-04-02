using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS.Areas.Identity.Data;
using POS.Models;

namespace POS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SalariesController : Controller
    {
        private readonly POSContext _context;

        public SalariesController(POSContext context)
        {
            _context = context;
        }

        // GET: Salaries
        public async Task<IActionResult> Index()
        {
            var pOSContext = _context.Salaries.Include(s => s.Employee);
            return View(await pOSContext.ToListAsync());
        }

        // GET: Salaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.SalaryId == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // GET: Salaries/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View();
        }

        // POST: Salaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalaryId,EmployeeId,CashAdvance")] Salary salary)
        {
            if (ModelState.IsValid)
            {

                //check if employee have salary record
                var existingSalary = await _context.Salaries.FirstOrDefaultAsync(existingSalary => existingSalary.EmployeeId == salary.EmployeeId);

                if (existingSalary != null)
                {

                    //subtract cash advance from grand total salary
                    existingSalary.GrandTotalSalary -= (existingSalary.CashAdvance ?? 0);
                    existingSalary.GrandTotalSalary -= (salary.CashAdvance ?? 0);

                    //update cash advance
                    existingSalary.CashAdvance = salary.CashAdvance;
                    _context.Update(existingSalary);
                }
                else
                {
                    salary.GrandTotalSalary -= (salary.CashAdvance ?? 0);
                    _context.Add(salary);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Email", salary.EmployeeId);
            return View(salary);
        }

        // GET: Salaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries.FindAsync(id);
            if (salary == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Email", salary.EmployeeId);
            return View(salary);
        }

        // POST: Salaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SalaryId,EmployeeId,CashAdvance")] Salary salary)
        {
            if (id != salary.SalaryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSalary = await _context.Salaries.FirstOrDefaultAsync(s => s.EmployeeId == salary.EmployeeId);

                    if (existingSalary != null)
                    {
                        // Add back the previous cash advance (restore deducted amount)
                        existingSalary.GrandTotalSalary += (existingSalary.CashAdvance ?? 0);

                        // Subtract the new cash advance
                        existingSalary.GrandTotalSalary -= (salary.CashAdvance ?? 0);

                        // Update the cash advance
                        existingSalary.CashAdvance = salary.CashAdvance;

                        _context.Update(existingSalary);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryExists(salary.SalaryId))
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

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Email", salary.EmployeeId);
            return View(salary);
        }


        // GET: Salaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.SalaryId == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // POST: Salaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salary = await _context.Salaries.FindAsync(id);
            if (salary != null)
            {
                _context.Salaries.Remove(salary);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Update Salary Status
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int salaryId, string status)
        {
            var salary = await _context.Salaries.FindAsync(salaryId);
            if (salary != null)
            {
                salary.Status = status; // Update status
                _context.Update(salary);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        private bool SalaryExists(int id)
        {
            return _context.Salaries.Any(e => e.SalaryId == id);
        }
    }
}
