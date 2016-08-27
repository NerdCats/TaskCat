namespace TaskCat.Data.Model.Vendor.ProfitSharing
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonKnownTypes(typeof(FlatRateStrategy))]
    public abstract class ProfitSharingStrategy
    {
        internal abstract protected ProfitSharingMethod Method { get; }

        internal protected abstract decimal Calculate(decimal totalPrice);

        public ProfitSharingStrategy()
        {
        }
    }
}