using System;

namespace FileSync
{
    public class FirebaseUploadException : Exception
    {
        public FirebaseUploadException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}