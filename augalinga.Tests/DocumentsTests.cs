using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using augalinga.Data.Entities;
using augalinga.Data.Access;
using augalinga.Backend.ViewModels;
using Xunit;

namespace augalinga.Tests
{
    public class DocumentsTests
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
        public void LoadDocuments_ShouldLoadAllDocumentsForProject()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            context.Documents.AddRange(
                new Document { Id = 1, Name = "Doc 1", Link = "link1", ProjectId = projectId },
                new Document { Id = 2, Name = "Doc 2", Link = "link2", ProjectId = projectId },
                new Document { Id = 3, Name = "Doc 3", Link = "link3", ProjectId = 2 } // Different project
            );
            context.SaveChanges();

            var viewModel = new DocumentsViewModel(projectId, context);

            // Act
            viewModel.LoadDocuments(projectId);

            // Assert
            Assert.Equal(2, viewModel.Documents.Count);
            Assert.Contains(viewModel.Documents, d => d.Name == "Doc 1");
            Assert.Contains(viewModel.Documents, d => d.Name == "Doc 2");
            Assert.DoesNotContain(viewModel.Documents, d => d.Name == "Doc 3");
        }

        [Fact]
        public void RemoveDocument_ShouldRemoveDocumentFromDatabaseAndReload()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            var documentToRemove = new Document { Id = 1, Name = "Doc to Remove", Link = "link-to-remove", ProjectId = projectId };
            context.Documents.AddRange(
                documentToRemove,
                new Document { Id = 2, Name = "Doc 2", Link = "link2", ProjectId = projectId }
            );
            context.SaveChanges();

            var viewModel = new DocumentsViewModel(projectId, context);

            // Act
            viewModel.RemoveDocument("link-to-remove");

            // Assert
            Assert.Single(viewModel.Documents);
            Assert.DoesNotContain(viewModel.Documents, d => d.Link == "link-to-remove");

            var documentsInDb = context.Documents.ToList();
            Assert.Single(documentsInDb);
            Assert.DoesNotContain(documentsInDb, d => d.Link == "link-to-remove");
        }

        [Fact]
        public void RemoveDocument_ShouldHandleNonExistentDocumentGracefully()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            context.Documents.AddRange(
                new Document { Id = 1, Name = "Doc 1", Link = "link1", ProjectId = projectId },
                new Document { Id = 2, Name = "Doc 2", Link = "link2", ProjectId = projectId }
            );
            context.SaveChanges();

            var viewModel = new DocumentsViewModel(projectId, context);

            // Act
            viewModel.RemoveDocument("non-existent-link");

            // Assert
            Assert.Equal(2, viewModel.Documents.Count);
            Assert.DoesNotContain(viewModel.Documents, d => d.Link == "non-existent-link");

            var documentsInDb = context.Documents.ToList();
            Assert.Equal(2, documentsInDb.Count);
        }
    }
}
