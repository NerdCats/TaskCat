namespace TaskCat.Model.Storage
{
    using System.IO;

    /// <summary>
    /// Model to download a file from a Storage Service
    /// </summary>

    public class FileDownloadModel
    {
        /// <summary>
        /// Blob stream of the file
        /// </summary>
        public MemoryStream BlobStream { get; set; }
        /// <summary>
        /// Filename that is used to save the file in the blob
        /// </summary>
        public string BlobFileName { get; set; }
        /// <summary>
        /// Content Type of the blob
        /// </summary>
        public string BlobContentType { get; set; }
        /// <summary>
        /// Blob Length
        /// </summary>
        public long BlobLength { get; set; }
    }
}