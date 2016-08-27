namespace TaskCat.Data.Model.Vendor
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonKnownTypes(typeof(FlatRateProfitSharingPreference))]
    public abstract class ProfitSharingPreference
    {
        internal abstract protected ProfitSharingMethod Method { get; }

        internal protected abstract decimal Calculate(decimal totalPrice);

        public ProfitSharingPreference()
        {
        }
    }
}