namespace TaskCat.Lib.Email
{
    public abstract class SendMailRequest
    {
        public string RecipientName { get; set; }
    }

    public class SendEmailInvoiceRequest: SendMailRequest
    {
        public Data.Entity.Job Job { get; set; }
        public string RecipientEmail { get; internal set; }
    }

}