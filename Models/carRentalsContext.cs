using Microsoft.EntityFrameworkCore;

namespace carRentals.Models
{
    public class carRentalsContext : DbContext
    {
        public carRentalsContext(DbContextOptions<carRentalsContext> options) : base(options)
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Rental> Rentals { get; set; }
    }
}