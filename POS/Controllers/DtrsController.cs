using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS.Areas.Identity.Data;
using POS.Models;

namespace POS.Controllers
{
    [Authorize(Roles = "Admin")]
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName");
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
        public async Task<IActionResult> Create([Bind("DtrId,EmployeeId,DtrDate,TimeIn,TimeOut")] Dtr dtr)
        {
            if (ModelState.IsValid)
            {
                //calculate total hours
                if(dtr.TimeOut < dtr.TimeIn)
                {
                    dtr.TimeOut = dtr.TimeOut.Value.AddDays(1);
                }

                TimeSpan timeWorked = (TimeSpan)(dtr.TimeOut.Value - dtr.TimeIn);
                double hours = Math.Round(timeWorked.TotalHours, 2);
                dtr.Hours = hours;
                double totalHours = dtr.Hours = hours;

                //get employee salary
                var employee = await _context.Employees.Include(employee => employee.Position)
                    .FirstOrDefaultAsync(employee => employee.EmployeeId == dtr.EmployeeId);

                if (employee == null)
                {
                    ModelState.AddModelError("", "Employee Not Found");
                    return View(dtr);
                }

                //calculate salary
                decimal salary = employee.Position?.Salary ?? 0;
                decimal totalSalary = (decimal)totalHours * salary;

                //Apply deduction to the full time employees
                if (employee.employmentType == "FullTime")
                {
                    decimal sss = totalSalary * 0.045m; //4.5%
                    decimal pagibig = totalSalary * 0.02m; //2%
                    decimal philhealth = totalSalary * 0.035m; //3.5%
                    decimal totalDeduction = sss + pagibig + philhealth;

                    totalSalary -= totalDeduction;

                }

                //check if employee have salary record
                var salaryRecord = await _context.Salaries.FirstOrDefaultAsync(salary => salary.EmployeeId == dtr.EmployeeId && salary.Status == "Unpaid");

                if (salaryRecord != null)
                {
                    //update employee salary
                    salaryRecord.GrandTotalHours = Math.Round(salaryRecord.GrandTotalHours + totalHours, 2);
                    salaryRecord.GrandTotalSalary = Math.Round(salaryRecord.GrandTotalSalary + totalSalary, 2);
                } else
                {
                    //create new salary for employee
                    salaryRecord = new Salary
                    {
                        EmployeeId = dtr.EmployeeId,
                        GrandTotalHours = totalHours,
                        GrandTotalSalary = totalSalary,
                        CashAdvance = 0,
                        Status = "Unpaid"
                    };
                    _context.Salaries.Add(salaryRecord);
                }

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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", dtr.EmployeeId);
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
                    //Get Original Dtr
                    var originalDtr = await _context.Dtrs.AsNoTracking().FirstOrDefaultAsync(d => d.DtrId == id);
                    if (originalDtr == null)
                    {
                        return NotFound();
                    }

                    //Calc Timeworked
                    if (originalDtr.TimeOut < originalDtr.TimeIn)
                    {
                        originalDtr.TimeOut = originalDtr.TimeOut.Value.AddDays(1);
                    }

                    TimeSpan dtrTimeWorked = (TimeSpan)(originalDtr.TimeOut.Value - originalDtr.TimeIn);
                    double dtrHours = Math.Round(dtrTimeWorked.TotalHours, 2);

                    var employee = await _context.Employees.Include(employee => employee.Position)
                        .FirstOrDefaultAsync(employee => employee.EmployeeId == dtr.EmployeeId);

                    if (employee == null)
                    {
                        ModelState.AddModelError("", "Employee Not Found");
                        return View(dtr);
                    }

                    decimal salaryRate = employee.Position?.Salary ?? 0;
                    decimal dtrTotalSalary = (decimal)dtrHours * salaryRate;

                    if (employee.employmentType == "FullTime")
                    {
                        decimal sss = dtrTotalSalary * 0.045m;
                        decimal pagibig = dtrTotalSalary * 0.02m;
                        decimal philhealth = dtrTotalSalary * 0.035m;
                        decimal totalDeduction = sss + pagibig + philhealth;

                        dtrTotalSalary -= totalDeduction;
                    }

                    //Calculate new time worked
                    if (dtr.TimeOut < dtr.TimeIn)
                    {
                        dtr.TimeOut = dtr.TimeOut.Value.AddDays(1);
                    }
                    TimeSpan newTimeWorked = (TimeSpan)(dtr.TimeOut.Value - dtr.TimeIn);
                    double newHours = Math.Round(newTimeWorked.TotalHours, 2);
                    double TotalHours = dtr.Hours = newHours;

                    decimal newTotalSalary = (decimal)newHours * salaryRate;

                    if (employee.employmentType == "FullTime")
                    {
                        decimal sss = newTotalSalary * 0.045m;
                        decimal pagibig = newTotalSalary * 0.02m;
                        decimal philhealth = newTotalSalary * 0.035m;
                        decimal totalDeduction = sss + pagibig + philhealth;

                        newTotalSalary -= totalDeduction;
                    }

                    //Update Salary  Record
                    var salaryRecord = await _context.Salaries.FirstOrDefaultAsync(salary => salary.EmployeeId == dtr.EmployeeId && salary.Status == "Unpaid");

                    if (salaryRecord != null)
                    {
                        salaryRecord.GrandTotalHours = Math.Round(salaryRecord.GrandTotalHours - dtrHours + newHours, 2);
                        salaryRecord.GrandTotalSalary = Math.Round(salaryRecord.GrandTotalSalary - dtrTotalSalary + newTotalSalary, 2);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Record already paid");
                        return RedirectToAction(nameof(Index));
                    }


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
            if (dtr == null)
            {
                return NotFound();
            }

            TimeSpan dtrTimeWorked = (TimeSpan)(dtr.TimeOut - dtr.TimeIn);
            double dtrHours = dtrTimeWorked.TotalMinutes / 60;

            var employee = await _context.Employees.Include(employee => employee.Position)
                .FirstOrDefaultAsync(employee => employee.EmployeeId == dtr.EmployeeId);

            if (employee == null)
            {
                ModelState.AddModelError("", "Employee Not Found!");
                return View(dtr);
            }

            decimal salaryRate = employee.Position?.Salary ?? 0;
            decimal dtrTotalSalary = (decimal)dtrHours * salaryRate;

            if (employee.employmentType == "FullTime")
            {
                decimal sss = dtrTotalSalary * 0.045m;
                decimal pagibig = dtrTotalSalary * 0.02m;
                decimal philhealth = dtrTotalSalary * 0.035m;
                decimal totalDeduction = sss + pagibig + philhealth;

                dtrTotalSalary -= totalDeduction;
            }

            //Update Salary Record
            var salaryRecord = await _context.Salaries.FirstOrDefaultAsync(salary => salary.EmployeeId == dtr.EmployeeId && salary.Status == "Unpaid");

            if(salaryRecord != null)
            {
                salaryRecord.GrandTotalHours = Math.Round(salaryRecord.GrandTotalHours - dtrHours, 2);
                salaryRecord.GrandTotalSalary = Math.Round(salaryRecord.GrandTotalSalary - dtrTotalSalary, 2);
            } else
            {
                ModelState.AddModelError("", "Record already paid");
                return View(dtr);
            }

                _context.Remove(dtr);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DtrExists(int id)
        {
            return _context.Dtrs.Any(e => e.DtrId == id);
        }
    }
}
