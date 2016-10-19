namespace TaskCat.Lib.Storage
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Model.Storage;
    using Constants;

    internal class StorageRepository : IStorageRepository
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

        public async Task<FileUploadModel> UploadImage(HttpContent content)
        {
            var fileUploadModel = await blobService.UploadBlob(content, "image", AppConstants.SupportedImageFormats);
            return fileUploadModel;
        }
    }
}