
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace augalinga.Backend.Models
{
    public class AzureBlobStorage
    {
        private readonly BlobServiceClient _blobServiceClient;
        private const string ContainerName = "augalinga";
        private string connectionString = Environment.GetEnvironmentVariable("AZURE_BLOB_CONNECTION_STRING");

        public AzureBlobStorage()
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> GetBlobUrlAsync(string projectName, string folder, string fileName)
        {
            var blobClient = _blobServiceClient.GetBlobContainerClient(ContainerName)
                .GetBlobClient($"{projectName}/{folder}/{fileName}");
            return blobClient.Uri.ToString();
        }

        public string GetBlobBaseUrl()
        {
            return _blobServiceClient.GetBlobContainerClient(ContainerName).Uri.ToString();
        }

        public async Task<bool> UploadFileAsync(string projectName, string blobName, Stream fileStream)
        {
            try
            {
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
                await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                var blobClient = blobContainerClient.GetBlobClient(blobName);

                await blobClient.UploadAsync(fileStream, true);
                Console.WriteLine($"Successfully uploaded {blobName} to {ContainerName}.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not upload {blobName} to {ContainerName}. Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<string>> ListBlobsAsync(string projectName)
        {
            var urls = new List<string>();
            try
            {
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
                await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync(prefix: $"{projectName}/"))
                {
                    var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                    urls.Add(blobClient.Uri.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error encountered while listing blobs: {ex.Message}");
            }

            return urls;
        }

        public async Task DeleteBlobAsync(string blobName)
        {
            try
            {
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
                var blobClient = blobContainerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
                Console.WriteLine($"Deleted {blobName} from {ContainerName}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting blob: {ex.Message}");
            }
        }

        public async Task DeletePhotoAsync(string projectName, string category, string fileName)
        {
            try
            {
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
                var blobClient = blobContainerClient.GetBlobClient($"{projectName}/photos/{category}/{fileName}");
                await blobClient.DeleteIfExistsAsync();
                Console.WriteLine($"Deleted {fileName} from {category} in {projectName}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting blob: {ex.Message}");
            }
        }

        public async Task RenameBlobAsync(string sourceBlobPath, string destinationBlobPath)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

            var sourceBlobClient = blobContainerClient.GetBlobClient(sourceBlobPath);
            var destinationBlobClient = blobContainerClient.GetBlobClient(destinationBlobPath);

            if (await sourceBlobClient.ExistsAsync())
            {
                using var downloadStream = await sourceBlobClient.OpenReadAsync();

                await destinationBlobClient.UploadAsync(downloadStream, overwrite: true);

                await sourceBlobClient.DeleteAsync();
            }
            else
            {
                throw new Exception("Source blob does not exist.");
            }
        }

        public async Task UploadCoverPhoto(string projectName, IBrowserFile file)
        {
            string blobName = $"{projectName}/cover photo/{file.Name}";
            using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10 MB limit
            await UploadFileAsync(projectName, blobName, stream);
        }

        public async Task UploadFile(string projectName, string folder, IBrowserFile file)
        {
            string blobName = $"{projectName}/{folder}/{file.Name}";
            using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10 MB limit
            await UploadFileAsync(projectName, blobName, stream);
        }

        public async Task UploadPhoto(string projectName, string category, IBrowserFile file)
        {
            string blobName = $"{projectName}/photos/{category}/{file.Name}";
            using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10 MB limit
            await UploadFileAsync(projectName, blobName, stream);
        }
    }
}
