namespace TaskCat.Lib.Email.SMTP
{
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using App.Settings;
    using Its.Configuration;
    using System.Net;
    using FluentEmail;
    using App_Start;
    using Model;

    public class SMTPMailService : IEmailService, IDisposable
    {
        private SMTPMailSettings settings;
        private SmtpClient smtpclient;
        private ProprietorSettings propSettings;
        private IFluentEmail mailFluent;

        public SMTPMailService()
        {
            settings = Settings.Get<SMTPMailSettings>();
            propSettings = Settings.Get<ProprietorSettings>();
            smtpclient = new SmtpClient(settings.Host, settings.Port)
            {
                Credentials = new NetworkCredential(settings.Username, settings.Password),
                EnableSsl = settings.EnableSSL
            };

            mailFluent = Email
               .From(settings.Username, propSettings.Name)
               .UsingClient(smtpclient);
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
            var email = mailFluent
            .To(request.RecipientEmail)
           .Subject("Welcome to " + propSettings.Name)
           .UsingTemplate(EmailTemplatesConfig.WelcomeEmailTemplate, new WelcomeEmail()
           {
               Name = request.RecipientUsername,
               ConfirmationUrl = request.ConfirmationUrl
           });

            try
            {
                await smtpclient.SendMailAsync(email.Message);
                return new SendEmailResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new SendEmailResponse()
                {
                    Error = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }
    }
}