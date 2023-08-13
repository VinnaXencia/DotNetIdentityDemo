using DotNetBrushUp.Areas.Identity.Data;
using DotNetBrushUp.DataModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DotNetBrushUp.Data;

public class DotNetBrushUpDbContext : IdentityDbContext<ApplicationUser>
{
    public DotNetBrushUpDbContext()
    {
    }

    public DotNetBrushUpDbContext(DbContextOptions<DotNetBrushUpDbContext> options)
        : base(options)
    {
    }
    public DbSet<EmployeeDetail> EmployeeDetails { get; set; }
    public DbSet<ContactDataModel> ContactsDataModel { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<ContactDataModel>()
            .Property(c => c.ContactId)
            .UseIdentityColumn();
    }
}
