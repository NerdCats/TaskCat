namespace TaskCat.App_Start
{
    using Data.Model;
    using Job.Utility.Discriminator;
    using MongoDB.Bson.Serialization;

    public class BsonSerializerConfig
    {
        public static void Configure()
        {
            BsonSerializer.RegisterDiscriminatorConvention(typeof(OrderModel), new OrderModelDiscriminator());
            BsonSerializer.RegisterDiscriminatorConvention(typeof(JobTask), new JobTaskDiscriminator());
        }
    }
}