using System;
using System.IO;
using System.Threading.Tasks;
using FileSync.Exceptions;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace FileSync
{
    public class FirebaseStorage : IFileUploader
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public FirebaseStorage(string credentialsFile, string bucketName)
        {
            _bucketName = bucketName;
            
            var credentials = GoogleCredential.FromFile(credentialsFile);
            _storageClient = StorageClient.Create(credentials);
        }
        
        public async Task UploadFile(string tempFile)
        {
            try
            {
                var objectName = Path.GetFileName(tempFile);
                var objectPath = Path.GetFullPath(tempFile);
                await using var objectStream = File.OpenRead(objectPath);
                Console.WriteLine($"Urcam fisierul {tempFile}...");
                await _storageClient.UploadObjectAsync(_bucketName, objectName, null, objectStream);
            }
            catch (Exception e)
            {
                var errorMessage = $"An error occured while uploading file {tempFile}: {e}";
                Console.WriteLine(errorMessage); // todo: log properly
                throw new FirebaseUploadException(errorMessage, e);
            }
        }

        public async Task DownloadFile(string fileName, string downloadLocation)
        {
            try
            {
                var absoluteFileLocation = Path.GetFullPath(downloadLocation);
                await using var objectStream = File.OpenWrite(@$"{absoluteFileLocation}\{fileName}");
                Console.WriteLine($"Descarcam fisierul {fileName} in {downloadLocation}...");
                await _storageClient.DownloadObjectAsync(_bucketName, fileName, objectStream);
            }
            catch (Exception e)
            {
                var errorMessage = $"An error occured while downloading file {fileName} to {downloadLocation}: {e}";
                Console.WriteLine(errorMessage); // todo: log properly
                throw new FirebaseDownloadException(errorMessage, e);
            }
        }
    }
}