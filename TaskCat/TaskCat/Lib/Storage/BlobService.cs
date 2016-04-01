namespace TaskCat.Lib.Storage
{
    using Microsoft.WindowsAzure.Storage.Blob;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Utility;
    using TaskCat.Model.Storage;

    internal class BlobService : IBlobService
    {
        public BlobService()
        {

        }

        public async Task<bool> TryDeleteBlob(string blobName)
        {
            var container = BlobHelper.GetBlobContainer();
            var blob = container.GetBlockBlobReference(blobName);

            return await blob.DeleteIfExistsAsync();
        }

        public async Task<FileDeleteModel> DeleteBlob(string blobName)
        {
            try
            {
                return new FileDeleteModel() { FileName = blobName, Status = await TryDeleteBlob(blobName) ? DeletionStatus.SUCCESSFUL : DeletionStatus.FILENOTFOUND };
            }
            catch
            {
                return new FileDeleteModel() { FileName = blobName, Status = DeletionStatus.FAILED };
            }
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

        public async Task<FileUploadModel> UploadBlob(HttpContent httpContent, string filterPropertyName, IEnumerable<string> supportedFileTypes = null)
        {
            var MultiPartProvider = new MultipartFormDataStreamProvider(Path.GetTempPath());
            await httpContent.ReadAsMultipartAsync(MultiPartProvider).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    throw task.Exception;
                }
            });

            var fileData = MultiPartProvider.FileData.FirstOrDefault(x => x.Headers.ContentDisposition.Name == string.Concat("\"", filterPropertyName, "\""));

            var blobContainer = BlobHelper.GetBlobContainer();

            if (fileData == null) return null;

            return await UploadOneToBlob(blobContainer, fileData, supportedFileTypes);
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

        private async Task<FileUploadModel> UploadOneToBlob(CloudBlobContainer blobContainer, MultipartFileData fileData, IEnumerable<string> supportedFileTypes = null)
        {
            var fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));

            if (supportedFileTypes != null)
                FileUtility.VerifyFileExtensionSupported(fileName, supportedFileTypes);

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