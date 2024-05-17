using ShoppingApi.InputData;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Cart;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Xml;
using ShoppingApi.WishList;
using ShoppingApi.Payment;
using ShoppingApi.Shipment;

namespace ShoppingApi.BusinessLogic
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<ShoppingInput> ShoppingInput { get; set;}
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<WishListItems> WishList { get; set; }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }
        public DbSet<ShipmentDetails> ShipmentDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WishListItems>()
                .HasIndex(e => e.WishListId)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

    }
}
