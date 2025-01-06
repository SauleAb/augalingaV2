using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using augalinga.Data.Entities;
using augalinga.Data.Enums;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
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

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DataContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                try
                {
                    var configuration = new ConfigurationBuilder()
                        .AddAzureKeyVault(new Uri("https://augalingakv.vault.azure.net/"), new DefaultAzureCredential())
                        .Build();

                    var connectionString = configuration["DefaultConnection"];

                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new Exception("Connection string 'DefaultConnection' was not found in Azure Key Vault.");
                    }

                    optionsBuilder.UseSqlServer(connectionString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error configuring DbContext: {ex.Message}");
                    throw;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>()
                .Property(n => n.Type)
                .HasConversion(new EnumToStringConverter<NotificationType>());

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany() 
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.ForUser)
                .WithMany() 
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
