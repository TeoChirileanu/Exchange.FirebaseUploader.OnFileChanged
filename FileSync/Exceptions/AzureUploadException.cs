using System;

namespace FileSync.Exceptions
{
    public class AzureUploadException : Exception
    {
        public AzureUploadException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}