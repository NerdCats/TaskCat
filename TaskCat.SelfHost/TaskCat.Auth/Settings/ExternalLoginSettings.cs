namespace TaskCat.Auth.Settings
{
    public class ExternalLoginSettings
    {
        public FacebookSettings Facebook { get; set; }
    }

    public class FacebookSettings
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}