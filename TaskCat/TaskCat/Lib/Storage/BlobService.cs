namespace TaskCat.Lib.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Model.Storage;

    internal class BlobService : IBlobService
    {
        public async Task<FileDownloadModel> DownloadBlob(string blobName)
        {
            var container = BlobHelper.GetBlobContainer();
            var blob = container.GetBlockBlobReference(blobName);

            var ms = new MemoryStream();
            await blob.DownloadToStreamAsync(ms);

            var lastPos = blob.Name.LastIndexOf('/');
            var fileName = blob.Name.Substring(lastPos + 1, blob.Name.Length - lastPos - 1);

            var downloadFileModel = new FileDownloadModel
            {
                BlobStream = ms,
                BlobFileName = fileName,
                BlobLength = blob.Properties.Length,
                BlobContentType = blob.Properties.ContentType
            };

            return downloadFileModel;
        }

        public async Task<List<FileUploadModel>> UploadBlobs(HttpContent httpContent)
        {
            var blobUploadProvider = new BlobStorageUploadProvider();
            var list = await httpContent.ReadAsMultipartAsync(blobUploadProvider).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    throw task.Exception;
                }

                var provider = task.Result;
                return provider.Uploads.ToList();
            });

            return list;
        }
    }
}