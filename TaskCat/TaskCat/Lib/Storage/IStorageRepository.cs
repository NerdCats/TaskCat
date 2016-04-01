namespace TaskCat.Lib.Storage
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Model.Storage;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IStorageRepository
    {
        Task<FileUploadModel> UploadImage(HttpContent content);
        Task<FileDeleteModel> DeleteImage(string fileName);
    }
}