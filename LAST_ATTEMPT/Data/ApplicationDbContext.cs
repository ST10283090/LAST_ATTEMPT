using LAST_ATTEMPT.Controllers;
using LAST_ATTEMPT.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LAST_ATTEMPT.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<AddToCart> AddToCarts { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
