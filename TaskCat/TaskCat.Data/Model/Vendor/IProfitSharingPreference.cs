namespace TaskCat.Data.Model.Vendor
{
    public interface IProfitSharingPreference
    {
        ProfitSharingMethod Method { get; }

        decimal Calculate(decimal totalPrice);
    }
}