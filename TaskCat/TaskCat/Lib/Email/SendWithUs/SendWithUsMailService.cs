﻿namespace TaskCat.Lib.Email.SendWithUs
{
    using App.Settings;
    using global::SendWithUs.Client;
    using Its.Configuration;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;


    public class SendWithUsMailService : IMailService
    {
        public SendWithUsMailService()
        {

        }

        public async Task<SendMailResponse> SendOrderMail(SendEmailInvoiceRequest request)
        {
            var proprietorSettings = Settings.Get<ProprietorSettings>();

            var settings = Settings.Get<SendWithUsSettings>();

            if (settings.Templates == null)
                throw new InvalidOperationException("SendWithUs settings doesn't have Templates dictionary");
            if (!settings.Templates.ContainsKey("OrderShip"))
                throw new InvalidOperationException("SendWithUs Templates dictionary doesn't have OrderShip template");

            var cultureInfo = new CultureInfo(proprietorSettings.CultureCode);

            SendRequest sendWithUsRequest = new SendRequest()
            {
                SenderName = proprietorSettings.Name,
                RecipientName = request.RecipientName,
                ProviderId = string.IsNullOrEmpty(settings.ProviderId) ? null : settings.ProviderId,
                RecipientAddress = request.RecipientEmail,
                TemplateId = settings.Templates["OrderShip"],
                Data = new OrderMail()
                {
                    Invoice = new EmailInvoice()
                    {
                        Items = request.Job.Order.OrderCart?.PackageList?.Select(x => new EmailInvoiceItem(x, proprietorSettings.CultureCode)).ToList(),
                        ServiceCharge = request.Job.Order.OrderCart.ServiceCharge.Value.ToString("C", cultureInfo),
                        ShippingAddress = request.Job.Order.To.FormattedAddress,
                        ShippingDate = DateTime.Now.ToShortDateString(),
                        SubTotal = request.Job.Order.OrderCart.SubTotal.Value.ToString("C", cultureInfo),
                        Total = request.Job.Order.OrderCart.TotalToPay.Value.ToString("C", cultureInfo),
                        VAT = request.Job.Order.OrderCart.TotalVATAmount.Value.ToString("C", cultureInfo)

                    },
                    JobId = request.Job.HRID,
                    Proprietor = proprietorSettings
                },
            };

            var client = new SendWithUsClient(settings.ApiKey);
            var response = await client.SendAsync(sendWithUsRequest);

            return new SendMailResponse()
            {
                Error = response.ErrorMessage,
                StatusCode = response.StatusCode,
                Success = response.Success
            };
        }
    }
}