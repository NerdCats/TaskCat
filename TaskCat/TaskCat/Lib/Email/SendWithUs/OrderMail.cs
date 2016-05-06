using TaskCat.App.Settings;

namespace TaskCat.Lib.Email.SendWithUs
{
    public class OrderMail
    {
        public EmailInvoice Invoice { get; set; }
        public string JobId { get; set; }
        public string AssetTrackUrl { get; set; }
        public ProprietorSettings Proprietor { get; set; }
    }
}