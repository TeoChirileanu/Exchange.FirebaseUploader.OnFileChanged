using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace FileSync.Tests
{
    public class AzureStorageTests
    {
        private readonly FileInfo _fileToUpload = new(@"Test\ToUpload.txt");
        private readonly ILogger _logger = Substitute.For<ILogger>();

        [SetUp]
        public void SetUp()
        {
            _fileToUpload.Directory?.Create();
            File.WriteAllText(_fileToUpload.FullName, "barfoo");
        }
        
        [Test]
        public async Task ShouldUploadFile()
        {
            // Arrange
            var connectionString = Environment.GetEnvironmentVariable("AZURE_CONNECTION_STRING");
            var azureStorage = new AzureStorage(connectionString!, _logger);

            // Act
            await azureStorage.UploadFile(_fileToUpload);

            // Assert
            Assert.Pass("Manual Check");
        }

        [TearDown]
        public void TearDown()
        {
            _fileToUpload.Delete();
            _fileToUpload.Directory?.Delete();
        }
    }
}