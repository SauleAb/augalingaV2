using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using augalinga.Data.Entities;
using augalinga.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Contact = augalinga.Data.Entities.Contact;

namespace augalinga.Data.Access
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Draft> Drafts { get; set; }
        public DbSet<Order> Orders {  get; set; }
        public DbSet<Expense> Expenses {  get; set; }
        public DbSet<Notification> Notifications {  get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=augalingaDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>()
                .Property(n => n.Type)
                .HasConversion(new EnumToStringConverter<NotificationType>());

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany() // Assuming User has many Notifications
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade); // This enables cascade delete

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.ForUser)
                .WithMany() // Assuming User has many Notifications as ForUser
                .HasForeignKey(n => n.ForUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Meetings)
                .WithMany(m => m.SelectedUsers)
                .UsingEntity(j => j.ToTable("UserMeetings"));

            base.OnModelCreating(modelBuilder);
        }
    }
}
