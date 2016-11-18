namespace TaskCat.Common.Storage
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Model.Storage;
    using System.Collections.Generic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IStorageRepository
    {
        Task<FileUploadModel> UploadImage(HttpContent content, IEnumerable<string> supportedImageFormats);
        Task<FileDeleteModel> DeleteImage(string fileName);
    }
}