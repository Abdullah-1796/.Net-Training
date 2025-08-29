// Data/AppDbContext.cs
using EF_Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace YourNamespace.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        ConfigureCustomEntities(mb);
        ConfigureIdentity(mb);
    }

    private void ConfigureCustomEntities(ModelBuilder mb)
    {
        // Ensure pgcrypto for gen_random_uuid() (we'll create it in a migration step)
        // Keys + default UUIDs
        mb.Entity<Room>(e =>
        {
            e.HasKey(x => x.RoomId);
            e.Property(x => x.RoomId).HasDefaultValueSql("gen_random_uuid()");
            e.Property(x => x.RoomNo).IsRequired();
            e.HasIndex(x => x.RoomNo).IsUnique();
        });

        mb.Entity<Customer>(e =>
        {
            e.HasKey(x => x.CustomerId);
            e.Property(x => x.CustomerId).HasDefaultValueSql("gen_random_uuid()");
            //e.Property(x => x.Cnic).IsRequired();
            //e.Property(x => x.Name).IsRequired();
            //e.Property(x => x.Phone).IsRequired();
            //e.Property(x => x.Email).IsRequired();

            e.HasIndex(x => x.Cnic).IsUnique();   // CNIC must be unique
            e.HasIndex(x => x.Email);              // optional, useful for lookup
        });

        mb.Entity<Employee>(e =>
        {
            e.HasKey(x => x.EmployeeId);
            e.Property(x => x.EmployeeId).HasDefaultValueSql("gen_random_uuid()");
            //e.Property(x => x.Cnic).IsRequired();
            //e.Property(x => x.Name).IsRequired();
            //e.Property(x => x.Phone).IsRequired();
            //e.Property(x => x.Email).IsRequired();
            //e.Property(x => x.Role).IsRequired();
            e.HasIndex(x => x.Cnic).IsUnique();   // CNIC must be unique
            e.HasIndex(x => x.Email);              // optional, useful for lookup
        });

        mb.Entity<Booking>(e =>
        {
            e.HasKey(x => x.BookingId);
            e.Property(x => x.BookingId).HasDefaultValueSql("gen_random_uuid()");

            // Map DateOnly to SQL 'date'
            e.Property(x => x.ExpectedCheckIn).HasColumnType("date");
            e.Property(x => x.CheckOut).HasColumnType("date");

            // Relationships
            e.HasOne(x => x.customer)
             .WithMany(c => c.Bookings)
             .HasForeignKey(x => x.CustomerId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.room)
             .WithMany(r => r.Bookings)
             .HasForeignKey(x => x.RoomId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Optional seed
        mb.Entity<Room>().HasData(
            new Room { RoomId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RoomNo = 101, Capacity = 2, Status = "Available" },
            new Room { RoomId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), RoomNo = 102, Capacity = 4, Status = "Available" }
        );
    }

    private void ConfigureIdentity(ModelBuilder mb)
    {
        //Rename default identity tables

        // Identity configuration
        mb.Entity<ApplicationUser>(b =>
        {
            b.ToTable("users");
            b.HasKey(u => u.Id);
            b.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
        });
        mb.Entity<ApplicationRole>(b =>
        {
            b.ToTable("roles");
            b.HasKey(r => r.Id);
            b.Property(r => r.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(r => r.Description).IsRequired(false);
        });
        mb.Entity<IdentityUserRole<Guid>>().ToTable("userroles");
        mb.Entity<IdentityUserClaim<Guid>>().ToTable("userclaims");
        mb.Entity<IdentityUserLogin<Guid>>().ToTable("userlogins");
        mb.Entity<IdentityRoleClaim<Guid>>().ToTable("roleclaims");
        mb.Entity<IdentityUserToken<Guid>>().ToTable("usertokens");

        //seed roles data
        Guid adminRoleId = Guid.Parse("c8d89a25-4b96-4f20-9d79-7f8a54c5213d");
        Guid managerRoleId = Guid.Parse("b92f0a3e-573b-4b12-8db1-2ccf6d58a34a");
        Guid receptionistRoleId = Guid.Parse("d7f4a42e-1c1b-4c9f-8a50-55f6b234e8e2");

        mb.Entity<ApplicationRole>().HasData(
            new ApplicationRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN", Description = "Administrator with full access" },
            new ApplicationRole { Id = managerRoleId, Name = "Manager", NormalizedName = "MANAGER", Description = "Manager with limited access" },
            new ApplicationRole { Id = receptionistRoleId, Name = "Receptionist", NormalizedName = "RECEPTIONIST", Description = "Receptionist with basic access" }
        );
    }
}

