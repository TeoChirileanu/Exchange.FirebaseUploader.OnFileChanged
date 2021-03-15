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

        public async Task UploadFile(FileInfo file)
        {
            try
            {
                var directory = file.Directory ?? throw new ArgumentNullException(nameof(file));
                _logger.LogInformation($"Uploading file {file.FullName}...");

                var directoryClient = _share.GetDirectoryClient(directory.Name);
                await directoryClient.CreateIfNotExistsAsync();

                var fileClient = directoryClient.GetFileClient(file.Name);
                await fileClient.CreateAsync(file.Length);
                await using var fileStream = file.OpenRead();
                await fileClient.UploadAsync(fileStream);

                _logger.LogInformation($"Successfully uploaded at {fileClient.Uri}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to upload {file} to {_share.Uri}: {e}");
            }
        }

        public async Task<bool> FileExists(FileInfo file)
        {
            try
            {
                var directory = file.Directory ?? throw new ArgumentNullException(nameof(file));
                var directoryClient = _share.GetDirectoryClient(directory.Name);
                var response = await directoryClient.ExistsAsync();
                var directoryExists = response.Value;
                if (!directoryExists)
                {
                    _logger.LogError($"Directory {directory} doesn't exist");
                    return false;
                }

                var fileClient = directoryClient.GetFileClient(file.Name);
                response = await fileClient.ExistsAsync();
                var fileExists = response.Value;
                return fileExists;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to check existence of {file}: {e}");
                return false;
            }
        }

        public async Task DeleteFile(FileInfo file)
        {
            try
            {
                var directory = file.Directory ?? throw new ArgumentNullException(nameof(file));
                var directoryClient = _share.GetDirectoryClient(directory.Name);
                var response = await directoryClient.ExistsAsync();
                var directoryExists = response.Value;
                if (!directoryExists)
                {
                    _logger.LogError($"Directory {directory} doesn't exist");
                    return;
                }
                var fileClient = directoryClient.GetFileClient(file.Name);
                await fileClient.DeleteIfExistsAsync();
                _logger.LogInformation($"Deleted file {fileClient.Uri}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed delete file {file}: {e}");
            }
        }

        public async Task DeleteDirectory(string directory)
        {
            var directoryClient = _share.GetDirectoryClient(directory);
            var response = await directoryClient.ExistsAsync();
            var directoryExists = response.Value;
            if (!directoryExists)
            {
                _logger.LogError($"Directory {directory} doesn't exist");
                return;
            }
            await directoryClient.DeleteIfExistsAsync();
            _logger.LogInformation($"Deleted directory {directoryClient.Uri}");
        }
    }
}