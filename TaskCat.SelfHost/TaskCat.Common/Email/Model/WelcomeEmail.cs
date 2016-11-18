namespace TaskCat.Common.Email.Model
{
    using Settings;

    public class WelcomeEmail : IEmailPayload
    {
        public string Name { get; set; }
        public string ConfirmationUrl { get; set; }
        public ProprietorSettings Proprietor { get; set; }
    }
}