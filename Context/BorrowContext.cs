using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MvcCF5.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using MvcCF5.Migrations;

namespace MvcCF5.Context
{
    public class BorrowContext : DbContext
    {
        public BorrowContext() : base("BorrowContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BorrowContext, Configuration>("BorrowContext"));

        }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ShopCart> ShopCarts { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Supplier>().Property(e => e.longtitude).HasPrecision(18,14);
            modelBuilder.Entity<Supplier>().Property(e => e.latitude).HasPrecision(18,14);
            modelBuilder.Entity<UserAccount>().Property(e => e.Latitude).HasPrecision(18,14);
            modelBuilder.Entity<UserAccount>().Property(e => e.Longtitude).HasPrecision(18,14);
            modelBuilder.Entity<Order>().Property(e => e.Latitude).HasPrecision(18,14);
            modelBuilder.Entity<Order>().Property(e => e.Longtitude).HasPrecision(18,14);
            base.OnModelCreating(modelBuilder);

        }

    }
}
