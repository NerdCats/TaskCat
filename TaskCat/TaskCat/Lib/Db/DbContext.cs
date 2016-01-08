namespace TaskCat.Lib.Db
{
    using MongoDB.Driver;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;

    public class DbContext : IDbContext
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private MongoClient mongoClient;

        public IMongoDatabase Database { get; private set; }

        public DbContext()
        {
            InitiateDatabase();
        }

        private void InitiateDatabase()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Mongo.ConnectionString"].ConnectionString;
            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            Database = mongoClient.GetDatabase(mongoUrlBuilder.DatabaseName);
        }
    }
}