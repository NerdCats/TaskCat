namespace TaskCat.Common.Settings
{
    using System.ComponentModel.DataAnnotations;
    public class ClientSettings
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "WebCatUrl is missing in ClientSettings")]
        public string WebCatUrl { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "ConfirmEmailPath is missing in ClientSettings")]
        public string ConfirmEmailPath { get; set; }
        public string HostingAddress { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "AuthenticationIssuerName is missing in ClientSettings")]
        public string AuthenticationIssuerName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "ServiceBusUrl is missing in ClientSettings")]
        public string ServiceBusUrl { get; set; }
    }
}