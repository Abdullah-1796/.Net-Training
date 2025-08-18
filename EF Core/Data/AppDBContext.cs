// Data/AppDbContext.cs
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
namespace YourNamespace.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
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
            e.Property(x => x.CheckIn).HasColumnType("date");
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
}
