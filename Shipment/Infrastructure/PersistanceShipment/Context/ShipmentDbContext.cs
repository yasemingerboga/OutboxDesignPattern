using DomainShipment;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceShipment.Context
{
    public class ShipmentDbContext : DbContext
    {
        public const string DEFATULT_SCHEMA = "shipmentdb";
        public ShipmentDbContext(DbContextOptions<ShipmentDbContext> options) : base(options)
        {

        }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentOutbox> ShipmentOutboxes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shipment>().ToTable("Shipments", DEFATULT_SCHEMA);
            modelBuilder.Entity<ShipmentOutbox>().ToTable("ShipmentOutboxes", DEFATULT_SCHEMA);
            modelBuilder.Entity<Shipment>().HasKey(p => p.Id);
            modelBuilder.Entity<ShipmentOutbox>().HasKey(p => p.IdempotentToken);
            base.OnModelCreating(modelBuilder);
        }
    }
}

