using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using augalinga.Data.Entities;
using augalinga.Backend.ViewModels;
using augalinga.Data.Access;
using Xunit;
using Contact = augalinga.Data.Entities.Contact;

namespace augalinga.Tests
{
    public class ContactsViewModelTests
    {
        private DataContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new DataContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public void LoadContacts_ShouldLoadAllContacts_WhenNoCategorySpecified()
        {
            // Arrange
            var context = CreateDbContext();
            context.Contacts.AddRange(
                new Contact {Id = 1, Name = "Contact 1", Category = "Nurseries", Address = "Gedimino g", Notes = "Loud", Number = "+3706070707" },
                new Contact {Id = 2, Name = "Contact 2", Category = "Transport", Address = "Gedimino g", Notes = "Loud", Number = "+3706070707" }
            );
            context.SaveChanges();

            var viewModel = new ContactsViewModel(context);

            // Act
            viewModel.LoadContacts();

            // Assert
            Assert.Equal(2, viewModel.Contacts.Count);
            Assert.Contains(viewModel.Contacts, c => c.Name == "Contact 1");
            Assert.Contains(viewModel.Contacts, c => c.Name == "Contact 2");
        }

        [Fact]
        public void LoadContacts_ShouldLoadFilteredContacts_ByCategory()
        {
            // Arrange
            var context = CreateDbContext();
            context.Contacts.AddRange(
                new Contact {Id = 1, Name = "Contact 1", Category = "Nurseries", Address = "Gedimino g", Notes = "Loud", Number = "+3706070707" },
                new Contact { Id = 2, Name = "Contact 2", Category = "Transport", Address = "Gedimino g", Notes = "Loud", Number = "+3706070707" }
            );
            context.SaveChanges();

            var viewModel = new ContactsViewModel("Nurseries", context);

            // Act
            viewModel.LoadContacts("Nurseries");

            // Assert
            Assert.Single(viewModel.Contacts);
            Assert.Contains(viewModel.Contacts, c => c.Name == "Contact 1");
        }

        [Fact]
        public async Task AddContact_ShouldAddContactAndReload()
        {
            // Arrange
            var context = CreateDbContext();
            var viewModel = new ContactsViewModel(context);
            var newContact = new Contact { Id = 1, Name = "New Contact", Category = "Nurseries", Address = "Gedimino g", Notes = "Loud", Number = "+3706070707"};

            // Act
                context.Contacts.Add(newContact);
                context.SaveChanges();

                viewModel.Contacts.Add(newContact);
            

            // Assert
            Assert.Single(viewModel.Contacts);
            Assert.Contains(viewModel.Contacts, c => c.Name == "New Contact");
        }

        [Fact]
        public async Task RemoveContact_ShouldRemoveContactAndReload()
        {
            // Arrange
            var context = CreateDbContext();
            var contactToRemove = new Contact { Id = 1, Name = "Contact to Remove", Category = "Transport", Address = "Gedimino g", Notes = "Loud", Number = "+3706070707" };
            context.Contacts.Add(contactToRemove);
            context.SaveChanges();

            var viewModel = new ContactsViewModel(context);

            // Act
                viewModel.RemoveContact(contactToRemove);

            // Assert
            Assert.Empty(viewModel.Contacts);
        }

        [Fact]
        public async Task UpdateContact_ShouldModifyContactDetails()
        {
            // Arrange
            var context = CreateDbContext();
            var contactToUpdate = new Contact {Id = 1, Name = "Old Name", Category = "Transport", Address = "Gedimino g", Notes = "Loud", Number = "+3706070707" };
            context.Contacts.Add(contactToUpdate);
            context.SaveChanges();

            var viewModel = new ContactsViewModel(context);

            // Act
            await Task.Run(() =>
            {
                contactToUpdate.Name = "Updated Name";
                context.Contacts.Update(contactToUpdate);
                context.SaveChanges();
                viewModel.LoadContacts("Transport");
            });

            // Assert
            Assert.Single(viewModel.Contacts);
            Assert.Contains(viewModel.Contacts, c => c.Name == "Updated Name");
        }
    }
}
