using DomainPayment.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Context
{
    public class OrderDbContext : DbContext
    {
        public const string DEFATULT_SCHEMA = "orderdb";
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {

        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderOutbox> OrderOutboxes { get; set; }
        public DbSet<OrderInbox> OrderInboxes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable("Orders", DEFATULT_SCHEMA);
            modelBuilder.Entity<OrderOutbox>().ToTable("OrderOutboxes", DEFATULT_SCHEMA);
            modelBuilder.Entity<OrderInbox>().ToTable("OrderInboxes", DEFATULT_SCHEMA);
            modelBuilder.Entity<Order>().HasKey(p => p.Id);
            modelBuilder.Entity<OrderOutbox>().HasKey(p => p.IdempotentToken);
            modelBuilder.Entity<OrderInbox>().HasKey(p => p.IdempotentToken);
            base.OnModelCreating(modelBuilder);
        }
    }
}
