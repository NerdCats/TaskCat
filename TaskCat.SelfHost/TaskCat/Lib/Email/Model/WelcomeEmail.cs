namespace TaskCat.Lib.Email.Model
{
    using App.Settings;
    public class WelcomeEmail : IEmailPayload
    {
        public string Name { get; set; }
        public string ConfirmationUrl { get; set; }
        public ProprietorSettings Proprietor { get; set; }
    }
}