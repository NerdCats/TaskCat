namespace TaskCat.Lib.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web;
    using Model.Storage;
    using System.IO;
    using System.Threading.Tasks;

    internal class BlobStorageUploadProvider : MultipartFileStreamProvider
    {
        public List<FileUploadModel> Uploads { get; set; }

        public BlobStorageUploadProvider() : base(Path.GetTempPath())
        {
            Uploads = new List<FileUploadModel>();
        }

        public override Task ExecutePostProcessingAsync()
        {
            foreach (var fileData in FileData)
            {
                var fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));
                var blobContainer = BlobHelper.GetBlobContainer();
                var blob = blobContainer.GetBlockBlobReference(fileName);

                blob.Properties.ContentType = fileData.Headers.ContentType.MediaType;

                using (var fs = File.OpenRead(fileData.LocalFileName))
                {
                    blob.UploadFromStream(fs);
                }

                // Delete local file from disk
                File.Delete(fileData.LocalFileName);

                var fileUpload = new FileUploadModel
                {
                    FileName = blob.Name,
                    FileUrl = blob.Uri.AbsoluteUri,
                    FileSizeInBytes = blob.Properties.Length
                };

                // Add uploaded blob to the list
                Uploads.Add(fileUpload);
            }

            return base.ExecutePostProcessingAsync();
        }
    }
}