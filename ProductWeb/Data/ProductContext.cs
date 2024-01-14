using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductWeb.Models;

namespace ProductWeb.Data
{
    public class ProductContext : IdentityDbContext
    {
        public ProductContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=.\\SqlExpress;Database=TestMVC67;Trusted_Connection=True; TrustServerCertificate = True");
        }

        public DbSet<User> Users {  get; set; }

    }
}
