using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NFluent;
using NSubstitute;
using NUnit.Framework;

namespace FileSync.Tests
{
    public class ReactiveFileWatcherTests
    {
        private readonly FileInfo _fileToWatch = new(@"Test\ToWatch.txt");
        private readonly ILogger _logger = Substitute.For<ILogger>();


        [SetUp]
        public void SetUp() {
            _fileToWatch.Directory?.Create();
            File.WriteAllText(_fileToWatch.FullName, string.Empty); // create the file
        }
        
        [Test]
        public async Task ShouldWriteHelloToWatchedFile_AfterItHasBeenModified() {
            // Arrange
            const string originalContent = "Hello";;

            async Task OnFileChanged(FileInfo _)
            {
                await File.WriteAllTextAsync(_fileToWatch.FullName, originalContent);
            }

            using var fileWatcher = new ReactiveFileWatcher(_fileToWatch, OnFileChanged, _logger);

            // Act
            await File.WriteAllTextAsync(_fileToWatch.FullName, string.Empty);
            Thread.Sleep(2 * fileWatcher.EventDelay);

            // Assert
            var actualContent = await File.ReadAllTextAsync(_fileToWatch.FullName);
            Check.That(actualContent).IsEqualTo(originalContent);
        }

        [TearDown]
        public void TearDown() {
            _fileToWatch.Delete();
            _fileToWatch.Directory?.Delete();
        }
    }
}