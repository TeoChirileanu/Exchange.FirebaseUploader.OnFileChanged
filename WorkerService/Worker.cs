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
        private readonly ILogger _logger;

        public Worker(ILogger logger)
        {
            _logger = logger;
            _logger.LogInformation("Service started");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var fileToWatch = Environment.GetEnvironmentVariable("FILE_TO_WATCH");
                    if (fileToWatch == null)
                    {
                        _logger.LogError("No file to be watched. Please set variable FILE_TO_WATCH");
                        break;
                    }
                    var connectionString = Environment.GetEnvironmentVariable("AZURE_CONNECTION_STRING");
                    if (connectionString == null)
                    {
                        _logger.LogError("No connection string found. Please set variable AZURE_CONNECTION_STRING");
                        break;
                    }

                    var azureStorage = new AzureStorage(connectionString, _logger);
                    using var _ = new ReactiveFileWatcher(new FileInfo(fileToWatch), azureStorage.UploadFile, _logger);
                    
                    await Task.CompletedTask;
                }
                catch (Exception e)
                {
                    _logger.LogError($"Got a nasty error: {e}");
                    break;
                }
            }
        }
    }
}