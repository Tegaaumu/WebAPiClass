using ShoppingApi.InputData;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ShoppingApi.BusinessLogic
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<ShoppingInput> ShoppingInput { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
