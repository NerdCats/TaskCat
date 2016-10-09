namespace TaskCat.Data.Model.Vendor.ProfitSharing
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonKnownTypes(typeof(FlatRateStrategy), typeof(PricePercentageStrategy))]
    public abstract class ProfitSharingStrategy
    {
        public ProfitSharingMethod Method { get; }

        public abstract ProfitShareResult Calculate(decimal totalPrice);
    }
}