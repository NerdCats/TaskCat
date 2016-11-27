namespace TaskCat.Common.Email.SMTP
{
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Net;
    using FluentEmail;
    using Settings;
    using Email;
    using Model;

    public class SMTPMailService : IEmailService, IDisposable
    {
        private SMTPMailSettings settings;
        private SmtpClient smtpclient;
        private ProprietorSettings propSettings;

        public SMTPMailService(SMTPMailSettings smtpMailSettings, ProprietorSettings proprietorSettings)
        {
            settings = smtpMailSettings;
            propSettings = proprietorSettings;
            smtpclient = new SmtpClient(settings.Host, settings.Port)
            {
                Credentials = new NetworkCredential(settings.Username, settings.Password),
                EnableSsl = settings.EnableSSL
            };
        }

        public void Dispose()
        {
            try
            {
                if (smtpclient != null)
                {
                    smtpclient.Dispose();
                }
            }
            catch (Exception) { }
        }

        public Task<SendEmailResponse> SendOrderMail(SendEmailInvoiceRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<SendEmailResponse> SendWelcomeMail(SendWelcomeEmailRequest request, string templatePath)
        {
            var email = Email
               .From(settings.Username, propSettings.Name)
               .UsingClient(smtpclient)
               .To(request.RecipientEmail)
               .Subject("Welcome to " + propSettings.Name)
               .UsingTemplate(templatePath, new WelcomeEmail()
               {
                   Name = request.RecipientUsername,
                   ConfirmationUrl = request.ConfirmationUrl,
                   Proprietor = propSettings
               });

            try
            {
                await smtpclient.SendMailAsync(email.Message);
                return new SendEmailResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new SendEmailResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}