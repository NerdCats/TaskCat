namespace TaskCat.Lib.Storage
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Model.Storage;

    /// <summary>
    /// Interface to define a Blob Storage Service
    /// </summary>
    public interface IBlobService
    {
        /// <summary>
        /// Upload single blob data from HttpContent
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filterPropertyName"></param>
        /// <param name="supportedFileTypes"></param>
        /// <returns></returns>              
        Task<FileUploadModel> UploadBlob(HttpContent content, string filterPropertyName, IEnumerable<string> supportedFileTypes = null);
        /// <summary>
        /// Upload Blobs data from HttpContent
        /// </summary>
        /// <param name="content"></param>
        /// <returns>
        /// </returns>
        Task<List<FileUploadModel>> UploadBlobs(HttpContent content);
        /// <summary>
        /// Download a file from Blob with a blob name
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task<FileDownloadModel> DownloadBlob(string blobName);
        /// <summary>
        /// Tries to delete a blob/file from storage
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<bool> TryDeleteBlob(string fileName);
        /// <summary>
        /// Deletes a blob/file from storage with proper status
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>
        /// Returns a FileDeleteModel object with proper deletion status
        /// </returns>
        Task<FileDeleteModel> DeleteBlob(string fileName);
    }
}