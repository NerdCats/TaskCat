namespace TaskCat.Lib.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Model.Storage;

    public class BlobService : IBlobService
    {
        public Task<FileDownloadModel> DownloadBlob(string blobName)
        {
            throw new NotImplementedException();
        }

        public Task<List<FileUploadModel>> UploadBlobs(HttpContent content)
        {
            throw new NotImplementedException();
        }
    }
}