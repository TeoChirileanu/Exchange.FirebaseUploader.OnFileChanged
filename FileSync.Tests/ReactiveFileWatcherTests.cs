using System;
using System.IO;
using System.Threading;
using NFluent;
using NUnit.Framework;

namespace FileSync.Tests
{
    public class ReactiveFileWatcherTests
    {
        private const string TestDirectoryName = "Test";
        private const string FileToWatchName = "ToWatch.txt";

        private DirectoryInfo _directoryToWatch = null!;
        private string _fileToWatch = null!;


        [SetUp]
        public void SetUp() {
            // create folder in current executing folder (usually bin/debug)
            _directoryToWatch = Directory.CreateDirectory(TestDirectoryName);
            
            // watch the file Test\ToWatch.txt
            _fileToWatch = Path.Combine(_directoryToWatch.FullName, FileToWatchName);
            
            // create the file by writing sth to to, then close
            File.WriteAllText(_fileToWatch, string.Empty);
        }
        
        [Test]
        public void ShouldWriteHelloToWatchedFile_AfterItHasBeenModified() {
            // Arrange
            const string originalContent = "Hello";;
            
            using var fileWatcher = new ReactiveFileWatcher(_fileToWatch, async _ =>
                await File.WriteAllTextAsync(_fileToWatch, originalContent));

            // Act
            File.WriteAllText(_fileToWatch, string.Empty); // change the watched file
            Thread.Sleep(2 * fileWatcher.EventDelay);

            // Assert
            var actualContent = File.ReadAllText(_fileToWatch);
            Check.That(actualContent).IsEqualTo(originalContent);
        }

        [TearDown]
        public void TearDown() {
            File.Delete(_fileToWatch);
            Directory.Delete(TestDirectoryName);
        }
    }
}