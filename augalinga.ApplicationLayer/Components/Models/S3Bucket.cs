using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.ObjectModel;

namespace augalinga.ApplicationLayer.Components.Models
{
    public class S3Bucket
    {
        public static async Task<bool> UploadFileAsync(
            IAmazonS3 client,
            string bucketName,
            string objectName,
            string filePath)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectName,
                FilePath = filePath,
            };

            var response = await client.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Successfully uploaded {objectName} to {bucketName}.");
                return true;
            }
            else
            {
                Console.WriteLine($"Could not upload {objectName} to {bucketName}.");
                return false;
            }
        }

        public static async Task<bool> UploadFilesAsync(IAmazonS3 client, string bucketName, IEnumerable<FileResult> objectNamesAndFilePaths)
        {
            try
            {
                foreach (var fileResult in objectNamesAndFilePaths)
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = fileResult.FileName, // Use file name as the key
                        FilePath = fileResult.FullPath,
                    };

                    var response = await client.PutObjectAsync(request);
                    if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine($"Could not upload {fileResult.FileName} to {bucketName}.");
                        return false;
                    }
                }

                Console.WriteLine("All files uploaded successfully.");
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server. Message: '{ex.Message}' while uploading files.");
                return false;
            }
        }

        public static async Task<bool> GetBucketPhotosAsync(IAmazonS3 client, string bucketName, ObservableCollection<byte[]> imageBytes)
        {
            try
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    MaxKeys = 5,
                };
                ListObjectsV2Response response;

                do
                {
                    response = await client.ListObjectsV2Async(request);

                    foreach (S3Object obj in response.S3Objects)
                    {
                        GetObjectRequest getObjectRequest = new GetObjectRequest
                        {
                            BucketName = bucketName,
                            Key = obj.Key
                        };

                        using (GetObjectResponse getObjectResponse = await client.GetObjectAsync(getObjectRequest))
                        using (Stream responseStream = getObjectResponse.ResponseStream)
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await responseStream.CopyToAsync(memoryStream);
                            byte[] bytes = memoryStream.ToArray();

                            imageBytes.Add(bytes);
                        }
                    }

                    request.ContinuationToken = response.NextContinuationToken;
                }
                while (response.IsTruncated);

                return true;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server. Message: '{ex.Message}' while getting list of objects.");
                return false;
            }
        }

        public static async Task<string> DownloadObjectAsync(IAmazonS3 s3Client, string bucketName, string keyName)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                using (GetObjectResponse response = await s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                {
                    string localFilePath = Path.Combine(Path.GetTempPath(), keyName);

                    string directoryPath = Path.GetDirectoryName(localFilePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    using (FileStream fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await responseStream.CopyToAsync(fileStream);
                    }

                    return localFilePath;
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when reading object", e.Message);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
                throw;
            }
        }

        public static async Task<bool> GetBucketDocumentsAsync(IAmazonS3 client, string bucketName, ObservableCollection<string> objectLinks)
        {
            try
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    MaxKeys = 5,
                };

                ListObjectsV2Response response;

                do
                {
                    response = await client.ListObjectsV2Async(request);

                    foreach (S3Object obj in response.S3Objects)
                    {
                        string link = $"https://{bucketName}.s3.amazonaws.com/{obj.Key}";

                        objectLinks.Add(link);
                    }

                    request.ContinuationToken = response.NextContinuationToken;
                }
                while (response.IsTruncated);

                return true;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server. Message: '{ex.Message}' while getting list of objects.");
                return false;
            }
        }

        public static async Task DeleteObject(IAmazonS3 client, string bucketName, string keyName)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                };

                await client.DeleteObjectAsync(deleteObjectRequest);
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server. Message:'{ex.Message}' when deleting an object.");
            }
        }

    }
}
