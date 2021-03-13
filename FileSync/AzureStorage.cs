using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Logging;

namespace FileSync
{
    public class AzureStorage
    {
        private const string ShareName = "exchange";
        private readonly ILogger _logger;
        private readonly ShareClient _share;

        public AzureStorage(string connectionString, ILogger logger)
        {
            _share = new ShareClient(connectionString, ShareName);
            _logger = logger;
        }

        public async Task UploadFile(FileInfo fileInfo)
        {
            try
            {
                _logger.LogInformation($"Uploading file {fileInfo.FullName}...");

                var directoryClient = _share.GetDirectoryClient(fileInfo.Directory!.Name);
                await directoryClient.CreateIfNotExistsAsync();

                var fileClient = directoryClient.GetFileClient(fileInfo.Name);
                await fileClient.CreateAsync(fileInfo.Length);
                await using var fileStream = fileInfo.OpenRead();
                await fileClient.UploadAsync(fileStream);

                _logger.LogInformation($"Successfully uploaded at {fileClient.Uri}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to upload {fileInfo} to {_share.Uri}: {e}");
            }
        }
    }
}