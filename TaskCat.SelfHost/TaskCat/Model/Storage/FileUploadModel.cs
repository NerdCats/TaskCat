namespace TaskCat.Model.Storage
{
    using System;

    /// <summary>
    /// Model to upload files to a Storage Service
    /// </summary>

    public class FileUploadModel
    {
        /// <summary>
        /// Desired File Name
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Url the file is being uploaded from
        /// </summary>
        public string FileUrl { get; set; }
        /// <summary>
        /// File Size in Bytes
        /// </summary>
        public long FileSizeInBytes { get; set; }
        /// <summary>
        /// File Size in Kilobytes
        /// </summary>
        public long FileSizeInKb { get { return (long)Math.Ceiling((double)FileSizeInBytes / 1024); } }
        /// <summary>
        /// File Size in Megabytes
        /// </summary>
        public long FileSizeInMb { get { return (long)Math.Ceiling((double)FileSizeInKb / 1024); } }
    }

}