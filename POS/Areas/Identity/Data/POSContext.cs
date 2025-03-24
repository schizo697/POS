using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POS.Areas.Identity.Data;
using System.Reflection.Emit;

namespace POS.Areas.Identity.Data;

public class POSContext : IdentityDbContext<ApplicationUser>
{
    public POSContext(DbContextOptions<POSContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<PositionSalary> PositionSalaries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<PositionSalary>()
        .Property(p => p.Salary)
        .HasPrecision(18, 2);

        base.OnModelCreating(builder);
    }
}
