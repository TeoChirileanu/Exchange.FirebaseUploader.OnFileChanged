using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FileSync
{
    public class ReactiveFileWatcher : IDisposable
    {
        public readonly TimeSpan EventDelay = TimeSpan.FromMilliseconds(200);
        private readonly IDisposable _disposable;
        private readonly FileSystemWatcher _fileWatcher;

        public ReactiveFileWatcher(string fileToWatch, Func<string, Task> onFileChanged) {
            var directoryToWatch = Path.GetDirectoryName(fileToWatch);
            _fileWatcher = new FileSystemWatcher(directoryToWatch!, Path.GetFileName(fileToWatch));

            _disposable = Observable.FromEventPattern(_fileWatcher, nameof(_fileWatcher.Changed))
                .Sample(TimeSpan.FromMilliseconds(200))// throttle a little bit
                .Select(data => ((FileSystemEventArgs) data.EventArgs).FullPath) // get path only 
                .Subscribe(async s => await onFileChanged(s));

            Console.WriteLine($"Urmarim fisierul {fileToWatch}...");
            _fileWatcher.EnableRaisingEvents = true; // start watching
        }

        public void Dispose() {
            _fileWatcher.Dispose();
            _disposable.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}