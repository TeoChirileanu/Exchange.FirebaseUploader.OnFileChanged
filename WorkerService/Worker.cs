using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileSync;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _logger.LogInformation("Service started");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var fileToWatchAsString = Environment.GetEnvironmentVariable("FILE_TO_WATCH");
            if (fileToWatchAsString == null)
            {
                _logger.LogError("No file to be watched. Please set variable FILE_TO_WATCH");
                return;
            }

            var fileToWatch = new FileInfo(fileToWatchAsString);

            var connectionString = Environment.GetEnvironmentVariable("AZURE_CONNECTION_STRING");
            if (connectionString == null)
            {
                _logger.LogError("No connection string found. Please set variable AZURE_CONNECTION_STRING");
                return;
            }

            var azureStorage = new AzureStorage(connectionString, _logger);
            using var _ = new ReactiveFileWatcher(fileToWatch, azureStorage.UploadFile, _logger);

            while (!stoppingToken.IsCancellationRequested) await Task.Delay(-1, stoppingToken);
        }
    }
}