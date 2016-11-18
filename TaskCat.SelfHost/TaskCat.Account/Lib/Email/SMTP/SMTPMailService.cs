namespace TaskCat.Account.Lib.Email.SMTP
{
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Its.Configuration;
    using System.Net;
    using FluentEmail;
    using Common.Settings;
    using Common.Email;
    using Common.Email.Model;
    using Settings;

    public class SMTPMailService : IEmailService, IDisposable
    {
        private SMTPMailSettings settings;
        private SmtpClient smtpclient;
        private ProprietorSettings propSettings;

        public SMTPMailService()
        {
            settings = Settings.Get<SMTPMailSettings>();
            propSettings = Settings.Get<ProprietorSettings>();
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

        public async Task<SendEmailResponse> SendWelcomeMail(SendWelcomeEmailRequest request)
        {
            var email = Email
               .From(settings.Username, propSettings.Name)
               .UsingClient(smtpclient)
               .To(request.RecipientEmail)
               .Subject("Welcome to " + propSettings.Name)
               .UsingTemplate(EmailTemplatesConfig.WelcomeEmailTemplate, new WelcomeEmail()
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