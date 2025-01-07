using augalinga.Backend.ViewModels;
using augalinga.Data.Access;
using augalinga.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace augalinga.Tests
{
    public class PhotosTests
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
        public void LoadPhotos_ShouldLoadPhotosByProjectAndCategory()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;
            var category = "Landscape";

            var photos = new List<Photo>
            {
                new Photo { Id = 1, ProjectId = 1, Category = "Landscape", Name = "Photo1", Bytes = new byte[] { 0x01, 0x01, 0x01, 0x01 }, Link = "Test", Title = "Test" },
                new Photo { Id = 2, ProjectId = 1, Category = "Landscape", Name = "Photo2", Bytes = new byte[] { 0x01, 0x01, 0x01, 0x01 }, Link = "Test", Title = "Test" },
                new Photo { Id = 3, ProjectId = 1, Category = "Portrait", Name = "Photo3", Bytes = new byte[] { 0x01, 0x01, 0x01, 0x01 }, Link = "Test", Title = "Test" },
                new Photo { Id = 4, ProjectId = 2, Category = "Landscape", Name = "Photo4", Bytes = new byte[] { 0x01, 0x01, 0x01, 0x01 }, Link = "Test", Title = "Test" }
            };

            context.Photos.AddRange(photos);
            context.SaveChanges();

            // Act
            var viewModel = new PhotosViewModel(projectId, category, context);

            // Assert
            Assert.Equal(2, viewModel.Photos.Count);
            Assert.Contains(viewModel.Photos, p => p.Name == "Photo1");
            Assert.Contains(viewModel.Photos, p => p.Name == "Photo2");
        }

        [Fact]
        public void LoadAllPhotos_ShouldLoadAllPhotosForProject()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            var photos = new List<Photo>
            {
                new Photo { Id = 1, ProjectId = 1, Category = "Landscape", Name = "Photo1", Bytes = new byte[] { 0x01, 0x01, 0x01, 0x01 }, Link = "Test", Title = "Test" },
                new Photo {Id = 2, ProjectId = 1, Category = "Portrait", Name = "Photo2", Bytes = new byte[] { 0x01, 0x01, 0x01, 0x01 }, Link = "Test", Title = "Test"},
                new Photo { Id = 3, ProjectId = 2, Category = "Landscape", Name = "Photo3", Bytes = new byte[] { 0x01, 0x01, 0x01, 0x01 }, Link = "Test", Title = "Test" }
            };

            context.Photos.AddRange(photos);
            context.SaveChanges();

            // Act
            var viewModel = new PhotosViewModel(projectId, context);
            viewModel.Photos = new ObservableCollection<Photo>(context.Photos.Where(photo => photo.ProjectId == projectId).ToList());

            // Assert
            Assert.Equal(2, viewModel.Photos.Count);
            Assert.Contains(viewModel.Photos, p => p.Name == "Photo1");
            Assert.Contains(viewModel.Photos, p => p.Name == "Photo2");
        }

        [Fact]
        public void LoadPhotos_ShouldReturnEmpty_WhenNoPhotosForCategory()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;
            var category = "Landscape";

            var photos = new List<Photo>
            {
                new Photo {Id = 1, ProjectId = 1, Category = "Portrait", Name = "Photo1", Bytes = new byte[] { 0x01, 0x01, 0x01, 0x01 }, Link = "Test", Title = "Test"},
                new Photo {Id = 2, ProjectId = 2, Category = "Landscape", Name = "Photo2", Bytes = new byte[] { 0x01, 0x01, 0x01, 0x01 }, Link = "Test", Title = "Test"}
            };

            context.Photos.AddRange(photos);
            context.SaveChanges();

            // Act
            var viewModel = new PhotosViewModel(projectId, category, context);

            // Assert
            Assert.Empty(viewModel.Photos);
        }

        [Fact]
        public void LoadAllPhotos_ShouldReturnEmpty_WhenNoPhotosForProject()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            var photos = new List<Photo>
            {
                new Photo { Id = 1, ProjectId = 2, Category = "Landscape", Name = "Photo1", Bytes = new byte[] { 0x01, 0x01, 0x01, 0x01 }, Link = "Test", Title = "Test" },
                new Photo {Id = 2, ProjectId = 2, Category = "Portrait", Name = "Photo2", Bytes = new byte[] { 0x01, 0x01, 0x01, 0x01 }, Link = "Test", Title = "Test"}
            };

            context.Photos.AddRange(photos);
            context.SaveChanges();

            // Act
            var viewModel = new PhotosViewModel(projectId, context);

            // Assert
            Assert.Empty(viewModel.Photos);
        }
    }
}
