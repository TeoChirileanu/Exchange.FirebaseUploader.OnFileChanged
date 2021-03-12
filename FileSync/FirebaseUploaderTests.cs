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
        private const string DownloadLocation = @".\Test";
        private readonly string _tempFile = Path.GetRandomFileName();

        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(DownloadLocation);
            File.WriteAllText(_tempFile, FileContent);
        }

        [Test]
        public async Task ShouldUploadFile()
        {
            // Arrange
            var firebaseUploader = new FirebaseUploader(CredentialsFile, BucketName);

            // Act
            await firebaseUploader.UploadFile(_tempFile);
            
            // Assert
            var downloadFile = firebaseUploader.DownloadFile(_tempFile, DownloadLocation);
            Check.ThatAsyncCode(() => downloadFile).DoesNotThrow();
        }

        [Test]
        public async Task ShouldDownloadFile()
        {
            // Arrange
            var firebaseUploader = new FirebaseUploader(CredentialsFile, BucketName);
            await firebaseUploader.UploadFile(_tempFile);

            // Act
            await firebaseUploader.DownloadFile(_tempFile, DownloadLocation);
            
            // Assert
            var actualFileContent = await File.ReadAllTextAsync($@"{DownloadLocation}\{_tempFile}");
            Check.That(actualFileContent).IsEqualTo(FileContent);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_tempFile);
            Directory.Delete(DownloadLocation, true);
        }
    }
}