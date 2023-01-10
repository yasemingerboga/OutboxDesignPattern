using DomainPayment;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentPersistance.Context
{
    public class PaymentDbContext : DbContext
    {
        public const string DEFATULT_SCHEMA = "paymentdb";
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {

        }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentOutbox> PaymentOutboxes { get; set; }
        public DbSet<PaymentInbox> PaymentInboxes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>().ToTable("Payments", DEFATULT_SCHEMA);
            modelBuilder.Entity<PaymentOutbox>().ToTable("PaymentOutboxes", DEFATULT_SCHEMA);
            modelBuilder.Entity<PaymentInbox>().ToTable("PaymentInboxes", DEFATULT_SCHEMA);
            modelBuilder.Entity<Payment>().HasKey(p => p.Id);
            modelBuilder.Entity<PaymentOutbox>().HasKey(p => p.IdempotentToken);
            modelBuilder.Entity<PaymentInbox>().HasKey(p => p.IdempotentToken);
            base.OnModelCreating(modelBuilder);
        }
    }
}
