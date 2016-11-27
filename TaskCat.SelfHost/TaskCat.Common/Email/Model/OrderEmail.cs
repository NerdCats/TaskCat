using TaskCat.Common.Settings;

namespace TaskCat.Common.Email.Model
{
    public class OrderEmail: IEmailPayload
    {
        public EmailInvoice Invoice { get; set; }
        public string JobId { get; set; }
        public string AssetTrackUrl { get; set; }
        public ProprietorSettings Proprietor { get; set; }
    }
}