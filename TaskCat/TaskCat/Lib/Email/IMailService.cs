namespace TaskCat.Lib.Email
{ 
    using System.Threading.Tasks;
    public interface IMailService
    {
        Task<SendMailResponse> SendOrderMail(SendEmailInvoiceRequest request);
    }
}