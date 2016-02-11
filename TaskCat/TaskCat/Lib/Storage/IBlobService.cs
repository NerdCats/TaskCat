namespace TaskCat.Lib.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using Model.Storage;

    /// <summary>
    /// Interface to define a Blob Storage Service
    /// </summary>
    public interface IBlobService
    {
        Task<List<FileUploadModel>> UploadBlobs(HttpContent content);
        Task<FileDownloadModel> DownloadBlob(string blobName);
    }
}