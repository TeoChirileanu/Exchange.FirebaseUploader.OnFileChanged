using System;

namespace FileSync
{
    public class FirebaseDownloadException : Exception
    {
        public FirebaseDownloadException(string errorMessage, Exception exception) : base(errorMessage, exception)
        {
        }
    }
}