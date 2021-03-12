using System;

namespace FileSync.Exceptions
{
    public class FirebaseDownloadException : Exception
    {
        public FirebaseDownloadException(string errorMessage, Exception exception) : base(errorMessage, exception)
        {
        }
    }
}