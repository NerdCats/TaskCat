namespace TaskCat.Account
{
    using System.Reflection;
    using AppSettings = Its.Configuration.Settings;
    using System.IO;
    using Settings;

    public static class EmailTemplatesConfig
    {
        public static string WelcomeEmailTemplate { get; set; }
        public static string OrderInvoiceEmailTemplate { get; set; }

        public static void Configure()
        {
            var settings = AppSettings.Get<EmailTemplatePathSettings>();

            // TODO: Need to fix this

            Assembly assembly = Assembly.GetExecutingAssembly();
            string path = System.IO.Path.GetDirectoryName(assembly.Location);

            using (TextReader reader = new StreamReader(string.Concat(path, "/App_Data/EmailTemplates/", nameof(settings.Welcome), ".html")))
            {
                WelcomeEmailTemplate = reader.ReadToEnd();
            }

            //TODO: Adding email templates here for razor templating for ourselves
            //Engine.Razor.AddTemplate(nameof(settings.Welcome), );
        }
    }
}