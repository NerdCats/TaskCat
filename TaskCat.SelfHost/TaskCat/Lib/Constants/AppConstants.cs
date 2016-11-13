namespace TaskCat.Lib.Constants
{
    public class AppConstants
    {
        public const string DefaultApiRoute = "DefaultApi";

        public const string JobsOdataRoute = "JobsOdata";
        public const string CommentOdataRoute = "CommentOdata";
        public const string DropPointOdataRoute = "DropPointOdata";
        public const string ProductCategoryRoute = "ProductCategoryOdata";
        public const string ProductOdataRoute = "ProductOdata";
        public const string StoreOdataRoute = "StoreOdata";
        public const string AccountOdataRoute = "AccountOdata";

        public const string DefaultCommentsRoute = "DefaultComments";
        public const string ConfirmEmailRoute = "ConfirmEmailRoute";
        public const string GetUserProfileByIdRoute = "GetUserProfileById";

        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 50;

        public static readonly string[] SupportedImageFormats = { ".jpg", ".png", ".PNG", "JPG"};
        public const int DefaultAssetSearchLimit = 10;

        public const int DefaultStoreOrder = 999;

        public const string DefaultHostingAddress = "http://localhost:8177/";
    }
}