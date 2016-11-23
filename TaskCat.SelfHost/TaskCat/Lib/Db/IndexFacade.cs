namespace TaskCat.Lib.Db
{
    using MongoDB.Driver;
    using Job = Data.Entity.Job;
    using System.Collections.Generic;
    using Data.Entity;
    using Data.Entity.Identity;
    using System;

    public class IndexFacade
    {
        public static void EnsureUniqueIndexOnPhoneNumber(IMongoCollection<User> userCollection)
        {
            CreateIndexOptions<User> options = new CreateIndexOptions<User>();
            options.Unique = true;
            options.Sparse = true;

            userCollection.Indexes.CreateOne(Builders<User>.IndexKeys.Ascending(x => x.PhoneNumber), options);
        }
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

            jobCollection.Indexes.CreateOne(Builders<Job>.IndexKeys.Geo2DSphere(x => x.Order.OrderLocation), geoIndexOptions);
        }

        public static void EnsureJobActivityIndexes(IMongoCollection<JobActivity> jobActivityCollection)
        {
            var jobActivityIndexOptions = new CreateIndexOptions();
            jobActivityIndexOptions.Background = true;

            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Ascending(x => x.JobId) ,jobActivityIndexOptions);
            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Ascending(x => x.Operation), jobActivityIndexOptions);
            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Ascending(x => x.ForUser.Id), jobActivityIndexOptions);
            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Ascending(x => x.ForUser.Username), jobActivityIndexOptions);
            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Ascending(x => x.TimeStamp), jobActivityIndexOptions);

            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Descending(x => x.JobId), jobActivityIndexOptions);
            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Descending(x => x.Operation), jobActivityIndexOptions);
            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Descending(x => x.ForUser.Id), jobActivityIndexOptions);
            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Descending(x => x.ForUser.Username), jobActivityIndexOptions);
            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Descending(x => x.TimeStamp), jobActivityIndexOptions);

            var referenceIndexOptions = new CreateIndexOptions();
            referenceIndexOptions.Background = true;
            referenceIndexOptions.Sparse = true;

            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Descending(x => x.Reference.EntityType), jobActivityIndexOptions);
            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Descending(x => x.Reference.Id), jobActivityIndexOptions);

            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Ascending(x => x.Reference.EntityType), jobActivityIndexOptions);
            jobActivityCollection.Indexes.CreateOne(Builders<JobActivity>.IndexKeys.Ascending(x => x.Reference.Id), jobActivityIndexOptions);
        }

        public static void EnsureHRIDIndex(IMongoCollection<HRIDEntity> hridCollection)
        {
            var hridIndexOptions = new CreateIndexOptions();
            hridIndexOptions.Unique = true;

            hridCollection.Indexes.CreateOne(Builders<HRIDEntity>.IndexKeys.Ascending(x=>x.HRID), hridIndexOptions);
            hridCollection.Indexes.CreateOne(Builders<HRIDEntity>.IndexKeys.Descending(x => x.HRID), hridIndexOptions);
        }

        public static void EnsureDropPointIndex(IMongoCollection<DropPoint> dropPointCollection)
        {
            dropPointCollection.Indexes.CreateOne(Builders<DropPoint>.IndexKeys.Ascending(x => x.Name));
            dropPointCollection.Indexes.CreateOne(Builders<DropPoint>.IndexKeys.Descending(x => x.Name));

            dropPointCollection.Indexes.CreateOne(Builders<DropPoint>.IndexKeys.Ascending(x => x.UserId));
            dropPointCollection.Indexes.CreateOne(Builders<DropPoint>.IndexKeys.Descending(x => x.UserId));

            dropPointCollection.Indexes.CreateOne(Builders<DropPoint>.IndexKeys.Descending(x => x.UserId));

            var combinedIndexOptions = new CreateIndexOptions();
            combinedIndexOptions.Unique = true;

            dropPointCollection.Indexes.CreateOne(Builders<DropPoint>.IndexKeys.Ascending(x => x.UserId).Ascending(x => x.Name), combinedIndexOptions);
            dropPointCollection.Indexes.CreateOne(Builders<DropPoint>.IndexKeys.Descending(x => x.UserId).Descending(x => x.Name), combinedIndexOptions);


            var geoIndexOptions = new CreateIndexOptions();
            geoIndexOptions.Background = true;
            geoIndexOptions.Sparse = true;

            dropPointCollection.Indexes.CreateOne(Builders<DropPoint>.IndexKeys.Geo2DSphere(x => x.Address.Point), geoIndexOptions);

        }

        public static void EnsureProductCategoriesIndex(IMongoCollection<ProductCategory> productCategoryCollection)
        {
            var UniqueIndexOptions = new CreateIndexOptions();
            UniqueIndexOptions.Unique = true;

            productCategoryCollection.Indexes.CreateOne(Builders<ProductCategory>.IndexKeys.Ascending(c => c.Name), UniqueIndexOptions);
            productCategoryCollection.Indexes.CreateOne(Builders<ProductCategory>.IndexKeys.Descending(c => c.Name), UniqueIndexOptions);
        }

        internal static void EnsureCommentIndexes(IMongoCollection<Comment> commentCollection)
        {
            var SparseIndexOptions = new CreateIndexOptions();
            SparseIndexOptions.Sparse = true;

            commentCollection.Indexes.CreateOne(Builders<Comment>.IndexKeys.Ascending(x => x.RefId));
            commentCollection.Indexes.CreateOne(Builders<Comment>.IndexKeys.Descending(x => x.RefId));
            commentCollection.Indexes.CreateOne(Builders<Comment>.IndexKeys.Ascending(x => x.RefCommentId), SparseIndexOptions);
            commentCollection.Indexes.CreateOne(Builders<Comment>.IndexKeys.Descending(x => x.RefCommentId), SparseIndexOptions);
            commentCollection.Indexes.CreateOne(Builders<Comment>.IndexKeys.Ascending(x => x.EntityType));
            commentCollection.Indexes.CreateOne(Builders<Comment>.IndexKeys.Descending(x => x.EntityType));
            commentCollection.Indexes.CreateOne(Builders<Comment>.IndexKeys.Ascending(x => x.CreateTime));
            commentCollection.Indexes.CreateOne(Builders<Comment>.IndexKeys.Descending(x => x.CreateTime));
            commentCollection.Indexes.CreateOne(Builders<Comment>.IndexKeys.Ascending(x => x.LastModified));
            commentCollection.Indexes.CreateOne(Builders<Comment>.IndexKeys.Descending(x => x.LastModified));
        }

        public static void EnsureVendorIndex(IMongoCollection<Vendor> vendorProfileCollection)
        {
            var UniqueIndexOptions = new CreateIndexOptions();
            UniqueIndexOptions.Unique = true;

            vendorProfileCollection.Indexes.CreateOne(Builders<Vendor>.IndexKeys.Ascending(x => x.UserId), UniqueIndexOptions);
            vendorProfileCollection.Indexes.CreateOne(Builders<Vendor>.IndexKeys.Descending(x => x.UserId), UniqueIndexOptions);
        }
    }
}