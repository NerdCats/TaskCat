namespace TaskCat.Lib.Catalog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using MongoDB.Driver;
    using Data.Entity;
    using Db;

    public class ProductCategoryService : IProductCategoryService
    {
        public IMongoCollection<ProductCategory> Collection { get; set; }

        public ProductCategoryService(IDbContext dbContext)
        {
            this.Collection = dbContext.ProductCategories;
        }

        public Task<ProductCategory> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductCategory> Get(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductCategory> Insert(ProductCategory obj)
        {
            throw new NotImplementedException();
        }

        public Task<ProductCategory> Update(ProductCategory obj)
        {
            throw new NotImplementedException();
        }
    }
}