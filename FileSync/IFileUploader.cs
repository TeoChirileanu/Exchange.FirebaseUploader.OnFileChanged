namespace FileSync
{
    public interface IFileUploader
    {
        void UploadFile(string absoluteFilePath);
    }
}