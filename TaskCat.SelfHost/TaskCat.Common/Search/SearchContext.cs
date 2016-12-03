namespace TaskCat.Common.Search
{
    using Nest;
    using System;
    using Settings;
    using AppSettings = Its.Configuration.Settings;

    public class SearchContext : ISearchContext
    {
        public ElasticClient Client { get; set; }

        public SearchContext()
        {
            InitiateConnection();
        }

        private void InitiateConnection()
        {
            var connectionUrl = new Uri(AppSettings.Get<ElasticSearchSettings>().ConnectionString);
            var settings = new ConnectionSettings(connectionUrl);
            this.Client = new ElasticClient(settings);
        }
    }
}
