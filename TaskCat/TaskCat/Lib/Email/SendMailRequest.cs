namespace TaskCat.Lib.Email
{
    public class SendMailRequest
    {
        public string RecipientUsername { get; set; }
        public string RecipientEmail { get; internal set; }
    }
    public class SendEmailInvoiceRequest : SendMailRequest
    {
        public Data.Entity.Job Job { get; set; }
    }
}