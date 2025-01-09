using augalinga.Backend.Models;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Dasync.Collections;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace augalinga.Tests
{
    public class AzureBlobStorageTests
    {
        private readonly Mock<BlobServiceClient> _blobServiceClientMock;
        private readonly Mock<BlobContainerClient> _blobContainerClientMock;
        private readonly Mock<BlobClient> _blobClientMock;

        public AzureBlobStorageTests()
        {
            _blobServiceClientMock = new Mock<BlobServiceClient>();
            _blobContainerClientMock = new Mock<BlobContainerClient>();
            _blobClientMock = new Mock<BlobClient>();

            _blobServiceClientMock
                .Setup(s => s.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(_blobContainerClientMock.Object);

            _blobContainerClientMock
                .Setup(c => c.GetBlobClient(It.IsAny<string>()))
                .Returns(_blobClientMock.Object);
        }

        [Fact]
        public async Task UploadFileAsync_ShouldReturnTrue_WhenUploadSucceeds()
        {
            // Arrange
            var storage = new AzureBlobStorage(_blobServiceClientMock.Object);
            var stream = new MemoryStream();

            _blobClientMock
                .Setup(b => b.UploadAsync(It.IsAny<Stream>(), true, default))
                .ReturnsAsync(Response.FromValue(Mock.Of<BlobContentInfo>(), Mock.Of<Response>()));

            // Act
            var result = await storage.UploadFileAsync("TestProject", "testBlob", stream);

            // Assert
            Assert.True(result);
            _blobClientMock.Verify(b => b.UploadAsync(It.IsAny<Stream>(), true, default), Times.Once);
        }

        [Fact]
        public async Task UploadFileAsync_ShouldReturnFalse_WhenUploadFails()
        {
            // Arrange
            var storage = new AzureBlobStorage(_blobServiceClientMock.Object);
            var stream = new MemoryStream();

            _blobClientMock
                .Setup(b => b.UploadAsync(It.IsAny<Stream>(), true, default))
                .ThrowsAsync(new Exception("Upload failed"));

            // Act
            var result = await storage.UploadFileAsync("TestProject", "testBlob", stream);

            // Assert
            Assert.False(result);
            _blobClientMock.Verify(b => b.UploadAsync(It.IsAny<Stream>(), true, default), Times.Once);
        }

        [Fact]
        public async Task GetBlobUrlAsync_ShouldReturnCorrectUrl()
        {
            // Arrange
            var storage = new AzureBlobStorage(_blobServiceClientMock.Object);
            var blobUrl = new Uri("https://test.blob.core.windows.net/container/blob");

            _blobClientMock
                .Setup(b => b.Uri)
                .Returns(blobUrl);

            // Act
            var result = await storage.GetBlobUrlAsync("TestProject", "testFolder", "testFile");

            // Assert
            Assert.Equal(blobUrl.ToString(), result);
            _blobContainerClientMock.Verify(c => c.GetBlobClient(It.IsAny<string>()), Times.Once);
        }
        

        [Fact]
        public async Task DeleteBlobAsync_ShouldDeleteBlob()
        {
            // Arrange
            var storage = new AzureBlobStorage(_blobServiceClientMock.Object);

            _blobClientMock
                .Setup(b => b.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, default))
                .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

            // Act
            await storage.DeleteBlobAsync("testBlob");

            // Assert
            _blobClientMock.Verify(b => b.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, default), Times.Once);
        }

        [Fact]
        public async Task RenameBlobAsync_ShouldRenameBlob_WhenSourceExists()
        {
            // Arrange
            var storage = new AzureBlobStorage(_blobServiceClientMock.Object);
            var stream = new MemoryStream();

            _blobClientMock
                .Setup(b => b.ExistsAsync(default))
                .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

            _blobClientMock
                .Setup(b => b.OpenReadAsync(new BlobOpenReadOptions(false), default))
                .ReturnsAsync(stream);

            _blobClientMock
                .Setup(b => b.UploadAsync(It.IsAny<Stream>(), true, default))
                .ReturnsAsync(Response.FromValue(Mock.Of<BlobContentInfo>(), Mock.Of<Response>()));

            // Act
            await storage.RenameBlobAsync("sourceBlob", "destinationBlob");

            // Assert
            _blobClientMock.Verify(b => b.UploadAsync(It.IsAny<Stream>(), true, default), Times.Once);
            _blobClientMock.Verify(b => b.DeleteAsync(DeleteSnapshotsOption.None, null, default), Times.Once);
        }

        [Fact]
        public async Task RenameBlobAsync_ShouldThrowException_WhenSourceDoesNotExist()
        {
            // Arrange
            var storage = new AzureBlobStorage(_blobServiceClientMock.Object);

            _blobClientMock
                .Setup(b => b.ExistsAsync(default))
                .ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => storage.RenameBlobAsync("sourceBlob", "destinationBlob"));
        }

        [Fact]
        public async Task ListBlobsAsync_ShouldReturnEmptyList_WhenNoBlobsExist()
        {
            // Arrange
            var storage = new AzureBlobStorage(_blobServiceClientMock.Object);

            var emptyPage = Page<BlobItem>.FromValues(new List<BlobItem>(), null, Mock.Of<Response>());
            var emptyPages = AsyncPageable<BlobItem>.FromPages(new[] { emptyPage });

            _blobContainerClientMock
                .Setup(c => c.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), default))
                .Returns(emptyPages);

            // Act
            var result = await storage.ListBlobsAsync("EmptyProject");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task UploadFileAsync_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var storage = new AzureBlobStorage(_blobServiceClientMock.Object);
            var stream = new MemoryStream();

            _blobClientMock
                .Setup(b => b.UploadAsync(It.IsAny<Stream>(), true, default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await storage.UploadFileAsync("TestProject", "testBlob", stream);

            // Assert
            Assert.False(result);
            _blobClientMock.Verify(b => b.UploadAsync(It.IsAny<Stream>(), true, default), Times.Once);
        }

        [Fact]
        public async Task DeletePhotoAsync_ShouldDeletePhoto_WhenCalled()
        {
            // Arrange
            var storage = new AzureBlobStorage(_blobServiceClientMock.Object);

            _blobClientMock
                .Setup(b => b.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, default))
                .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

            // Act
            await storage.DeletePhotoAsync("TestProject", "category", "photo.jpg");

            // Assert
            _blobClientMock.Verify(b => b.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, default), Times.Once);
        }

        [Fact]
        public async Task DeletePhotoAsync_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var storage = new AzureBlobStorage(_blobServiceClientMock.Object);

            _blobClientMock
                .Setup(b => b.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            await storage.DeletePhotoAsync("TestProject", "category", "photo.jpg");

            // Assert
            _blobClientMock.Verify(b => b.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, default), Times.Once);
        }

        [Fact]
        public void GetBlobBaseUrl_ShouldReturnBaseUrl_WhenCalled()
        {
            // Arrange
            var storage = new AzureBlobStorage(_blobServiceClientMock.Object);
            var baseUrl = new Uri("https://test.blob.core.windows.net/container");

            _blobContainerClientMock
                .Setup(c => c.Uri)
                .Returns(baseUrl);

            // Act
            var result = storage.GetBlobBaseUrl();

            // Assert
            Assert.Equal(baseUrl.ToString(), result);
        }


    }
}
