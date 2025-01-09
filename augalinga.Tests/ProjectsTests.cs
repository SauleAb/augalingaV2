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
    public class ProjectsTests
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
        public void LoadProjects_ShouldLoadAllProjects()
        {
            // Arrange
            var context = CreateDbContext();
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "Project 1", ImageUrl = "Image1" },
                new Project { Id = 2, Name = "Project 2", ImageUrl = "Image2" }
            };

            context.Projects.AddRange(projects);
            context.SaveChanges();

            // Act
            var viewModel = new ProjectsViewModel(context);

            // Assert
            Assert.Equal(2, viewModel.Projects.Count);
            Assert.Contains(viewModel.Projects, p => p.Name == "Project 1");
            Assert.Contains(viewModel.Projects, p => p.Name == "Project 2");
        }

        [Fact]
        public void Projects_ShouldRaisePropertyChanged_WhenValueIsSet()
        {
            // Arrange
            var context = CreateDbContext();
            var viewModel = new ProjectsViewModel(context);
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ProjectsViewModel.Projects))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.Projects = new ObservableCollection<Project>
            {
                new Project { Id = 1, Name = "New Project", ImageUrl = "NewImage" }
            };

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void LoadProjects_ShouldLoadEmptyCollection_WhenNoProjectsExist()
        {
            // Arrange
            var context = CreateDbContext();

            // Act
            var viewModel = new ProjectsViewModel(context);

            // Assert
            Assert.Empty(viewModel.Projects);
        }
    }
}
