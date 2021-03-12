using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace FileSync
{
    public class FirebaseUploader
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public FirebaseUploader(string credentialsFile, string bucketName)
        {
            _bucketName = bucketName;
            
            var credentials = GoogleCredential.FromFile(credentialsFile);
            _storageClient = StorageClient.Create(credentials);
        }
        
        public async Task UploadFile(string tempFile)
        {
            var objectName = Path.GetFileName(tempFile);
            var objectPath = Path.GetFullPath(tempFile);
            await using var objectStream = File.OpenRead(objectPath);

            // todo: create wrapper around storage client
            await _storageClient.UploadObjectAsync(_bucketName, objectName, null, objectStream);
        }

        public async Task DownloadFile(string fileName, string downloadLocation)
        {
            var absoluteFileLocation = Path.GetFullPath(downloadLocation);
            await using var objectStream = File.OpenWrite(@$"{absoluteFileLocation}\{fileName}");

            // todo: create wrapper around storage client
            await _storageClient.DownloadObjectAsync(_bucketName, fileName, objectStream);
        }
    }
}