using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace FileSync
{
    public class FirebaseUploader
    {
        private readonly string _credentialsFile;
        private readonly string _bucketName;

        public FirebaseUploader(string credentialsFile, string bucketName)
        {
            _credentialsFile = credentialsFile;
            _bucketName = bucketName;
        }
        
        public async Task UploadFile(string tempFile)
        {
            var credentials = GoogleCredential.FromFile(_credentialsFile);
            var storageClient = await StorageClient.CreateAsync(credentials);
            
            var objectName = Path.GetFileNameWithoutExtension(tempFile);
            var objectPath = Path.GetFullPath(tempFile);
            await using var objectStream = File.OpenRead(objectPath);

            // todo: create wrapper around storage client
            await storageClient.UploadObjectAsync(_bucketName, objectName, null, objectStream);
        }
    }
}