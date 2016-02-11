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
        /// <summary>
        /// Upload Blob data from HttpContent
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<List<FileUploadModel>> UploadBlobs(HttpContent content);
        /// <summary>
        /// Download a file from Blob with a blob name
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task<FileDownloadModel> DownloadBlob(string blobName);
    }
}