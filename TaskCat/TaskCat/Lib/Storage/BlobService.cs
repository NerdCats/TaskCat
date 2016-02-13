namespace TaskCat.Lib.Storage
{
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Model.Storage;

    internal class BlobService : IBlobService
    {
        public BlobService()
        {

        }

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

        public async Task<FileUploadModel> UploadBlob(HttpContent httpContent, string filterPropertyName)
        { 
            var MultiPartProvider = new MultipartFormDataStreamProvider(Path.GetTempPath());
            await httpContent.ReadAsMultipartAsync(MultiPartProvider).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    throw task.Exception;
                }
            });

            var fileData = MultiPartProvider.FileData.FirstOrDefault(x => x.Headers.ContentDisposition.Name == filterPropertyName);

            var blobContainer = BlobHelper.GetBlobContainer();

            if (fileData == null) return null;

            return await UploadOneToBlob(blobContainer, fileData);
        }

        public async Task<List<FileUploadModel>> UploadBlobs(HttpContent httpContent)
        {
            var MultiPartProvider = new MultipartFormDataStreamProvider(Path.GetTempPath());
            await httpContent.ReadAsMultipartAsync(MultiPartProvider).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    throw task.Exception;
                }
            });

            var blobContainer = BlobHelper.GetBlobContainer();
            List<FileUploadModel> Uploads = new List<FileUploadModel>();

            foreach (var fileData in MultiPartProvider.FileData)
            {
                FileUploadModel fileUpload = await UploadOneToBlob(blobContainer, fileData);

                Uploads.Add(fileUpload);
            }

            return Uploads;
        }

        private async Task<FileUploadModel> UploadOneToBlob(CloudBlobContainer blobContainer, MultipartFileData fileData)
        {
            var fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));
            var blob = blobContainer.GetBlockBlobReference(fileName);

            blob.Properties.ContentType = fileData.Headers.ContentType.MediaType;

            using (var fs = File.OpenRead(fileData.LocalFileName))
            {
                await blob.UploadFromStreamAsync(fs);
            }

            File.Delete(fileData.LocalFileName);

            var fileUpload = new FileUploadModel
            {
                FileName = blob.Name,
                FileUrl = blob.Uri.AbsoluteUri,
                FileSizeInBytes = blob.Properties.Length
            };
            return fileUpload;
        }
    }
}