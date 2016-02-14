namespace TaskCat.App_Start
{
    using Data.Model;
    using Lib.Utility.Discriminator;
    using MongoDB.Bson.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Data.Entity;
    using Data.Model.JobTasks;

    public class BsonSerializerConfig
    {
        public static void Configure()
        {
            BsonSerializer.RegisterDiscriminatorConvention(typeof(OrderModel), new OrderModelDiscriminator());
            BsonSerializer.RegisterDiscriminatorConvention(typeof(FetchRideTask), new JobTaskDiscriminator()); 
            BsonSerializer.RegisterDiscriminatorConvention(typeof(JobTask), new JobTaskDiscriminator());
        }
    }
}