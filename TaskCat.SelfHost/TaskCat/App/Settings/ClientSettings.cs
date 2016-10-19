namespace TaskCat.App.Settings
{
    using System.ComponentModel.DataAnnotations;
    public class ClientSettings
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "WebCatUrl is missing in ClientSettings")]
        public string WebCatUrl { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "ConfirmEmailPath is missing in ClientSettings")]
        public string ConfirmEmailPath { get; set; }
        public string HostingAddress { get; set; }
    }
}