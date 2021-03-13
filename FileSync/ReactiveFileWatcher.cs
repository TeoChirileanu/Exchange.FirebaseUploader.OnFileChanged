using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FileSync
{
    public class ReactiveFileWatcher : IDisposable
    {
        private readonly IDisposable? _disposable;
        private readonly FileSystemWatcher? _fileWatcher;
        public readonly TimeSpan EventDelay = TimeSpan.FromMilliseconds(200);

        public ReactiveFileWatcher(FileInfo fileToWatch, Func<FileInfo, Task> onFileChanged, ILogger logger)
        {
            try
            {
                var directoryToWatch = fileToWatch.DirectoryName;
                _fileWatcher = new FileSystemWatcher(directoryToWatch!, fileToWatch.Name);

                _disposable = Observable.FromEventPattern(_fileWatcher, nameof(_fileWatcher.Changed))
                    .Sample(TimeSpan.FromMilliseconds(200)) // throttle a little bit
                    .Select(data => ((FileSystemEventArgs) data.EventArgs).FullPath)
                    .Subscribe(async file => await onFileChanged(new FileInfo(file)));

                _fileWatcher.EnableRaisingEvents = true;

                logger.LogInformation($"Watching file {fileToWatch.FullName} for changes...");
            }
            catch (Exception e)
            {
                logger.LogError($"Could not creat watcher: {e}");
                throw;
            }
        }

        public void Dispose()
        {
            _fileWatcher?.Dispose();
            _disposable?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}