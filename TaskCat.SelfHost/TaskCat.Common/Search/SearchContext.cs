namespace TaskCat.Common.Search
{
    using Nest;
    using System;
    using Settings;
    using AppSettings = Its.Configuration.Settings;

    public class SearchContext : ISearchContext
    {
        public ElasticClient Client { get; set; }
        public bool SearchEnabled { get; private set; }

        public SearchContext()
        {
            InitiateConnection();
        }

        private void InitiateConnection()
        {
            try
            {
                var searchSettings = AppSettings.Get<ElasticSearchSettings>();
                if (searchSettings == null)
                {
                    SearchEnabled = false;
                    return;
                }
                var connectionUrl = new Uri(searchSettings.ConnectionString);
                var settings = new ConnectionSettings(connectionUrl);
                this.Client = new ElasticClient(settings);
            }
            catch (Exception)
            {
                //TODO: Log problems here
            }
        }
    }
}
