using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TennisTM.Models;

namespace TennisTM.Data;

public class TennisTMDbContext : IdentityDbContext<User>
{
    public TennisTMDbContext(DbContextOptions<TennisTMDbContext> options)
        : base(options)
    {
    }

    private DbSet<User> users;

    public DbSet<User> GetUsers()
    {
        return users;
    }

    public void SetUsers(DbSet<User> value)
    {
        users = value;
    }

    public DbSet<Coach> Coaches { get; set; }
    public DbSet<Schedule> Schedules { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<Schedule>()
            .HasMany(x => x.Users)
            .WithMany(y => y.Schedules)
            .UsingEntity(z => z.ToTable("ScheduleUser"));
    }
}
