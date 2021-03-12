using System.Threading.Tasks;

namespace FileSync
{
    public interface IFileUploader
    {
       Task UploadFile(string filePath);
    }
}