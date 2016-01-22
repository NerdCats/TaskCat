namespace TaskCat.App_Start
{
    using Data.Entity.Assets;
    using Data.Model;
    using Lib.Utility.Discriminator;
    using Model.JobTasks;
    using MongoDB.Bson.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class BsonSerializerConfig
    {
        public static void Configure()
        {
            BsonSerializer.RegisterDiscriminatorConvention(typeof(FetchRideTask<Ride>), new JobTaskDiscriminator()); 
            BsonSerializer.RegisterDiscriminatorConvention(typeof(JobTask), new JobTaskDiscriminator());
        }
    }
}