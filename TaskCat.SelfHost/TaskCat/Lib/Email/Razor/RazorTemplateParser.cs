namespace TaskCat.Lib.Email.Razor
{
    using RazorEngine;
    using RazorEngine.Templating;
    using Model;
    using System;
    using Its.Configuration;
    using App.Settings;
    using Common.Email.Model;
    using Common.Email;

    public class RazorTemplateParser : IEmailTemplateParser
    {
        EmailTemplatePathSettings templatePathSettings;
        public RazorTemplateParser()
        {
            templatePathSettings = Settings.Get<EmailTemplatePathSettings>();
        }
        public string PopulateTemplate(string template, string templateKey, object payload)
        {
            var result = Engine.Razor.RunCompile(template, "temp", null, payload);
            return result;
        }

        public string PopulateEmail<T>(T payload) where T : IEmailPayload
        {
            if (payload.GetType() == typeof(WelcomeEmail))
            {
                var result = Engine.Razor.RunCompile(nameof(templatePathSettings.Welcome), payload.GetType(), payload);
                return result;
            }
            else
            {
                throw new NotImplementedException(string.Format("Email Template population for type {0} is not supported yet", payload.GetType()));
            }
        }
    }
}