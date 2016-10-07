namespace TaskCat.Lib.Email
{
    public class SendEmailRequest
    {
        public string RecipientUsername { get; set; }
        public string RecipientEmail { get; internal set; }
    }
    public class SendEmailInvoiceRequest : SendEmailRequest
    {
        public Data.Entity.Job Job { get; set; }
    }

    public class SendWelcomeEmailRequest : SendEmailRequest
    {
        public string ConfirmationUrl { get; set; }
    }
}