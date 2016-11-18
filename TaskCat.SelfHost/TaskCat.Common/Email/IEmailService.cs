namespace TaskCat.Common.Email
{ 
    using System.Threading.Tasks;
    public interface IEmailService
    {
        Task<SendEmailResponse> SendOrderMail(SendEmailInvoiceRequest request);
        Task<SendEmailResponse> SendWelcomeMail(SendWelcomeEmailRequest request, string templatePath);
    }
}