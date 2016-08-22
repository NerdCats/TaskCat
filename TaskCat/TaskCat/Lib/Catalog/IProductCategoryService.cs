namespace TaskCat.Lib.Catalog
{
    using Data.Entity;
    using Domain;

    public interface IProductCategoryService : IRepository<ProductCategory>
    {
    }
}