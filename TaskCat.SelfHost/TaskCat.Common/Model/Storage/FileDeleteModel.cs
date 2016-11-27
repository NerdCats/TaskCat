namespace TaskCat.Common.Model.Storage
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Model to delete a file in Storage Service
    /// </summary>
    public class FileDeleteModel
    {
        public string FileName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DeletionStatus Status { get; set; }
    }

    /// <summary>
    /// Represents status of file deletion from storage
    /// service
    /// </summary>
    public enum DeletionStatus
    {
        SUCCESSFUL,
        FAILED,
        FILENOTFOUND
    }
}