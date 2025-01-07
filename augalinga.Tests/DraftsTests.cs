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

namespace augalinga.Tests
{
    public class DraftsTests
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
        public void LoadDrafts_ShouldLoadAllDraftsForSpecificProject()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            var drafts = new List<Draft>
            {
                new Draft { Id = 1, ProjectId = 1, Name = "Draft 1", Link = "Link1" },
                new Draft { Id = 2, ProjectId = 1, Name = "Draft 2", Link = "Link2" },
                new Draft { Id = 3, ProjectId = 2, Name = "Draft 3", Link = "Link3" }
            };

            context.Drafts.AddRange(drafts);
            context.SaveChanges();

            // Act
            var viewModel = new DraftsViewModel(projectId, context);

            // Assert
            Assert.Equal(2, viewModel.Drafts.Count);
            Assert.Contains(viewModel.Drafts, d => d.Name == "Draft 1");
            Assert.Contains(viewModel.Drafts, d => d.Name == "Draft 2");
        }

        [Fact]
        public void Drafts_ShouldRaisePropertyChanged_WhenValueIsSet()
        {
            // Arrange
            var context = CreateDbContext();
            var viewModel = new DraftsViewModel(1, context);
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(DraftsViewModel.Drafts))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.Drafts = new ObservableCollection<Draft>
            {
                new Draft { Id = 1, ProjectId = 1, Name = "New Draft", Link = "NewLink" }
            };

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void LoadDrafts_ShouldLoadEmptyCollection_WhenNoDraftsExistForProject()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            // Act
            var viewModel = new DraftsViewModel(projectId, context);

            // Assert
            Assert.Empty(viewModel.Drafts);
        }
    }
}
