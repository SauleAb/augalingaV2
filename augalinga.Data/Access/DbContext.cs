using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using augalinga.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Contact = augalinga.Data.Entities.Contact;

namespace augalinga.Data.Access
{
    public class DataContext : DbContext
    {
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Draft> Drafts { get; set; }
        public DbSet<Order> Orders {  get; set; }
        public DbSet<Expense> Expenses {  get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=augalingaDB;User Id=sa;Password=augalinga;TrustServerCertificate=True;");
        }
    }
}
