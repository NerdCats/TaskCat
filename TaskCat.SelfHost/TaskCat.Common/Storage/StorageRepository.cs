namespace TaskCat.Common.Storage
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Model.Storage;
    using System.Collections.Generic;

    public class StorageRepository : IStorageRepository
    {
        private readonly IBlobService blobService;

        public StorageRepository(IBlobService blobService)
        {
            this.blobService = blobService;
        }

        public async Task<FileDeleteModel> DeleteImage(string fileName)
        {
            return await blobService.DeleteBlob(fileName);
        }

        public async Task<FileUploadModel> UploadImage(HttpContent content, IEnumerable<string> supportedImageFormats)
        {
            var fileUploadModel = await blobService.UploadBlob(content, "image", supportedImageFormats);
            return fileUploadModel;
        }
    }
}