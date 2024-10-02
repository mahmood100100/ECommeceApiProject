using Ecommerce.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<LocalUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderDetails>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<OrderDetails>()
                .HasIndex(e => new { e.OrderId, e.ProductId, e.Id })
                .IsUnique();

            modelBuilder.Entity<Categories>()
                .HasMany(c => c.products)
                .WithOne(p => p.categories)
                .HasForeignKey(p => p.CetegoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Orders>()
                .HasMany(o => o.orderDetails)
                .WithOne(od => od.orders)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Products> Products { get; set; }
        public DbSet<Orders> Orders { get; set; }

        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<LocalUser> LocalUser { get; set; }

        public DbSet<Categories> Categories { get; set; }
        //public DbSet<IdentityUser> Users { get; set; }
        //public DbSet<IdentityRole> Roles { get; set; }

    }
}
