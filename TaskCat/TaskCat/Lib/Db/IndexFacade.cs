namespace TaskCat.Lib.Db
{
    using MongoDB.Driver;
    using Job = Data.Entity.Job;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class IndexFacade
    {
        public static void EnsureJobIndexes(IMongoCollection<Job> jobCollection)
        {
            //Time Based indexes
            CreateIndexOptions<Job> options = new CreateIndexOptions<Job>();
            options.Background = true;

            List<CreateIndexModel<Job>> TimeIndexes = new List<CreateIndexModel<Job>>();

            var ascendCreateTime = new CreateIndexModel<Job>(Builders<Job>.IndexKeys.Ascending(x => x.CreateTime), options);
            TimeIndexes.Add(ascendCreateTime);

            var descendCreateTime = new CreateIndexModel<Job>(Builders<Job>.IndexKeys.Descending(x => x.CreateTime), options);
            TimeIndexes.Add(descendCreateTime);

            var ascendModifiedTime = new CreateIndexModel<Job>(Builders<Job>.IndexKeys.Ascending(x => x.ModifiedTime), options);
            TimeIndexes.Add(ascendModifiedTime);

            var descendModifiedTime = new CreateIndexModel<Job>(Builders<Job>.IndexKeys.Descending(x => x.ModifiedTime), options);
            TimeIndexes.Add(descendModifiedTime);

            jobCollection.Indexes.CreateMany(TimeIndexes);

            //JobState Index
            List<CreateIndexModel<Job>> BackgroundIndexes = new List<CreateIndexModel<Job>>();

            var ascendJobState = new CreateIndexModel<Job>(Builders<Job>.IndexKeys.Ascending(x=>x.State), options);
            BackgroundIndexes.Add(ascendCreateTime);
            var descendJobState = new CreateIndexModel<Job>(Builders<Job>.IndexKeys.Descending(x => x.State), options);
            BackgroundIndexes.Add(descendCreateTime);

            jobCollection.Indexes.CreateMany(BackgroundIndexes);

            //Geo Indexes
            var geoIndexOptions = new CreateIndexOptions();
            geoIndexOptions.Background = true;
            geoIndexOptions.Sparse = true;

            jobCollection.Indexes.CreateOne(Builders<Job>.IndexKeys.Geo2DSphere(x => x.UserLocation), geoIndexOptions);
        }
    }
}