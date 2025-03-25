using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POS.Areas.Identity.Data;
using POS.Models;
using System.Reflection.Emit;

namespace POS.Areas.Identity.Data;

public class POSContext : IdentityDbContext<ApplicationUser>
{
    public POSContext(DbContextOptions<POSContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Dtr> Dtrs { get; set; }
    public DbSet<Salary> Salaries { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
    }
}
