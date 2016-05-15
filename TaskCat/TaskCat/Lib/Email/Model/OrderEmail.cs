using TaskCat.App.Settings;

namespace TaskCat.Lib.Email.Model
{
    public class OrderEmail: IEmailPayload
    {
        public EmailInvoice Invoice { get; set; }
        public string JobId { get; set; }
        public string AssetTrackUrl { get; set; }
        public ProprietorSettings Proprietor { get; set; }
    }
}