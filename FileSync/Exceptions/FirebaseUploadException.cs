using System;

namespace FileSync.Exceptions
{
    public class FirebaseUploadException : Exception
    {
        public FirebaseUploadException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}