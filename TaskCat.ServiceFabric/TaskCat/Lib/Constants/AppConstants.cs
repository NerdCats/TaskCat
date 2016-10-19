namespace TaskCat.Lib.Constants
{
    public class AppConstants
    {
        public const string DefaultApiRoute = "DefaultApi";
        public const string DefaultOdataRoute = "DefaultOdata";
        public const string CommentOdataRoute = "CommentOdata";
        public const string DefaultCommentsRoute = "DefaultComments";
        public const string ConfirmEmailRoute = "ConfirmEmailRoute";
        public const string GetUserProfileByIdRoute = "GetUserProfileById";

        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 50;

        public static readonly string[] SupportedImageFormats = { ".jpg", ".png", ".PNG", "JPG"};
        public const int DefaultAssetSearchLimit = 10;

        public const int DefaultStoreOrder = 999;

        public const string DefaultHostingAddress = "http://localhost:80/";
    }
}