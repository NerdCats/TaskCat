namespace TaskCat.Common.Storage
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System.Configuration;

    public class BlobHelper
    {
        internal static CloudBlobContainer GetBlobContainer()
        {
            // Pull these from config
            var blobStorageConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];
            var blobStorageContainerName = ConfigurationManager.AppSettings["BlobStorageContainerName"];

            // Create blob client and return reference to the container
            var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(blobStorageContainerName);
            container.CreateIfNotExists();
            container.SetPermissions(
                    new BlobContainerPermissions
                    {
                        PublicAccess =
                    BlobContainerPublicAccessType.Blob
                    });
            return container;
        }
    }
}