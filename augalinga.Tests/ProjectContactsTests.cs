using augalinga.Backend.ViewModels;
using augalinga.Data.Access;
using augalinga.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xunit;
using Contact = augalinga.Data.Entities.Contact;

namespace augalinga.Tests
{
    public class ProjectContactsTests
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
        public void LoadContacts_ShouldLoadAllContactsForSpecificProject()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            var contacts = new List<Contact>
            {
                new Contact { Id = 1, ProjectId = 1, Name = "Contact 1", Address = "Address 1", Number = "123456789", Notes = "Test" },
                new Contact { Id = 2, ProjectId = 1, Name = "Contact 2", Address = "Address 2", Number = "987654321", Notes = "Test" },
                new Contact { Id = 3, ProjectId = 2, Name = "Contact 3", Address = "Address 3", Number = "111111111", Notes = "Test" }
            };

            context.Contacts.AddRange(contacts);
            context.SaveChanges();

            // Act
            var viewModel = new ProjectContactsViewModel(projectId, context);

            // Assert
            Assert.Equal(2, viewModel.Contacts.Count);
            Assert.Contains(viewModel.Contacts, c => c.Name == "Contact 1");
            Assert.Contains(viewModel.Contacts, c => c.Name == "Contact 2");
        }

        [Fact]
        public void Contacts_ShouldRaisePropertyChanged_WhenValueIsSet()
        {
            // Arrange
            var context = CreateDbContext();
            var viewModel = new ProjectContactsViewModel(1, context);
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ProjectContactsViewModel.Contacts))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.Contacts = new ObservableCollection<Contact>
            {
                new Contact { Id = 1, ProjectId = 1, Name = "New Contact", Address = "New Address", Number = "222222222" }
            };

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void LoadContacts_ShouldLoadEmptyCollection_WhenNoContactsExistForProject()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            // Act
            var viewModel = new ProjectContactsViewModel(projectId, context);

            // Assert
            Assert.Empty(viewModel.Contacts);
        }
    }
}
