using DomainNotification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceNotification.Context
{

    public class NotificationDbContext : DbContext
    {
        public const string DEFATULT_SCHEMA = "notificationdb";
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {

        }
        public DbSet<Notification> Notificaitons { get; set; }
        public DbSet<NotificationOutbox> NotificationOutboxes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>().ToTable("Notifications", DEFATULT_SCHEMA);
            modelBuilder.Entity<NotificationOutbox>().ToTable("NotificationOutboxes", DEFATULT_SCHEMA);
            modelBuilder.Entity<Notification>().HasKey(p => p.Id);
            modelBuilder.Entity<NotificationOutbox>().HasKey(p => p.IdempotentToken);
            base.OnModelCreating(modelBuilder);
        }
    }
}

