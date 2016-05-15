using TaskCat.App.Settings;

namespace TaskCat.Lib.Email.Model
{
    public class WelcomeEmail : IEmailPayload
    {
        public string Name { get; set; }
        public string ConfirmationUrl { get; set; }
        public ProprietorSettings Proprietor { get; set; }
    }
}