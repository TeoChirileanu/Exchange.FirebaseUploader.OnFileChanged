using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FileSync
{
    public class FirebaseUploaderTests
    {
        private readonly string _tempFile = Path.GetRandomFileName();

        [SetUp]
        public void SetUp() => File.WriteAllText(_tempFile, string.Empty);
        
        [Test]
        public async Task ShouldUploadFile()
        {
            // Arrange
            const string credentialsFile = @"C:\Users\teodo\Documents\Google\TransactMe-1a88359a2131.json";
            const string bucketName = "transactme-db.appspot.com";
            var firebaseUploader = new FirebaseUploader(credentialsFile, bucketName);

            // Act
            await firebaseUploader.UploadFile(_tempFile);
            
            // Assert
            Assert.Pass("Manually check the store");
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_tempFile);
        }
    }
}