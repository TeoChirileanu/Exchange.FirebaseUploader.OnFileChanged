using System.IO;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;

namespace FileSync
{
    public class FirebaseUploaderTests
    {
        private const string CredentialsFile = @"C:\Users\teodo\Documents\Google\TransactMe-1a88359a2131.json";
        private const string BucketName = "transactme-db.appspot.com";
        private const string FileContent = "foobar";
        private readonly string _tempFile = Path.GetRandomFileName();

        [SetUp]
        public void SetUp() => File.WriteAllText(_tempFile, FileContent);
        
        [Test]
        public async Task ShouldUploadFile()
        {
            // Arrange
            var firebaseUploader = new FirebaseUploader(CredentialsFile, BucketName);

            // Act
            await firebaseUploader.UploadFile(_tempFile);
            
            // Assert
            Assert.Pass("Manual check");
        }

        [Test]
        public async Task ShouldDownloadFile()
        {
            // Arrange
            var firebaseUploader = new FirebaseUploader(CredentialsFile, BucketName);
            await firebaseUploader.UploadFile(_tempFile);

            // Act
            const string downloadLocation = @".\Test";
            Directory.CreateDirectory(downloadLocation);
            await firebaseUploader.DownloadFile(_tempFile, downloadLocation);
            
            // Assert
            var actualFileContent = await File.ReadAllTextAsync($@"{downloadLocation}\{_tempFile}");
            Check.That(actualFileContent).IsEqualTo(FileContent);
            
            // Cleanup
            Directory.Delete(downloadLocation, true);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_tempFile);
        }
    }
}