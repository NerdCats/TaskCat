namespace TaskCat.Lib.Cataglog
{
    using Data.Entity;
    using Domain;
    using MongoDB.Driver;

    public interface IProductCategoryService : IRepository<ProductCategory>
    {
        IMongoCollection<ProductCategory> Collection { get; set; }
    }
}