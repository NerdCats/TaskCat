namespace TaskCat.Lib.Email
{
    using TaskCat.Data.Model;

    public abstract class SendMailRequest
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Template { get; set; }
        public string SenderName { get; set; }

        public string SenderAddress { get; set; }
        public string RecipientAddress { get; set; }

    }

    public class SendOrderMailRequest : SendMailRequest
    {
        public OrderModel Order { get; set; }
    }

}