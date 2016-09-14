namespace TaskCat.Data.Model.Vendor.ProfitSharing
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonKnownTypes(typeof(FlatRateStrategy), typeof(PricePercentageStrategy))]
    public abstract class ProfitSharingStrategy
    {
        internal abstract protected ProfitSharingMethod Method { get; }

        internal protected abstract ProfitShareResult Calculate(decimal totalPrice);
    }
}