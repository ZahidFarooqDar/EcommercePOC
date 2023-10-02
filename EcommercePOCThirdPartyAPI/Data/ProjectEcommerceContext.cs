using EcommercePOCThirdPartyAPI.DomainModals;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EcommercePOCThirdPartyAPI.Data
{
    public class ProjectEcommerceContext : DbContext
    {
        public ProjectEcommerceContext(DbContextOptions<ProjectEcommerceContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<Delivery> DeliveryInfos { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<TrackingNumber> TrackingNumber { get; set; }
        public DbSet<Tracker> Trackers { get; set; }
        public DbSet<Recipient> Recipient { get; set; }
        

        public DbSet<Order> Orders { get; set; }
        public DbSet<Event> Events { get; set; }
        //public DbSet<TrackerDM> Trackers { get; set; }
        public DbSet<TrackerEvent> TrackerEvent { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EcommercePOC;Trusted_Connection=True;MultipleActiveResultSets=true");
            //optionsBuilder.UseSqlServer("Server=192.168.29.71;Database=HeroAPIDB;User Id=sa;Password=123@Reno;MultipleActiveResultSets=true;Encrypt=False;");
            base.OnConfiguring(optionsBuilder);
            //For deployment we use below method for database creation
            /*optionsBuilder.UseSqlServer("Server=192.168.29.71;Database=HeroDB;User Id=sa;Password=123@Reno;MultipleActiveResultSets=true;Encrypt=False;");
            base.OnConfiguring(optionsBuilder);*/
        }
        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define relationships using Fluent API

            // Buyer-Seller Relationship (One-to-Many)
            modelBuilder.Entity<Seller>()
                .HasOne(s => s.Buyer)
                .WithMany(b => b.Sellers)
                .HasForeignKey(s => s.BuyerId);

            // Seller-Product Relationship (One-to-Many)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SellerId);

            // Buyer-ShippingAddress Relationship (One-to-Many)
            modelBuilder.Entity<ShippingAddress>()
                .HasOne(sa => sa.Buyer)
                .WithMany(b => b.ShippingAddresses)
                .HasForeignKey(sa => sa.BuyerId);
        }*/
      
    }
}

