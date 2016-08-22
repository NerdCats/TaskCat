namespace TaskCat.Lib.Catalog
{
    using Data.Entity;
    using Domain;
    using System;
    using System.Threading.Tasks;
    using MongoDB.Driver;

    public class IProductService : IRepository<Product>
    {
        public IMongoCollection<Product> Collection { get;  }

        public Task<Product> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> Get(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> Insert(Product obj)
        {
            throw new NotImplementedException();
        }

        public Task<Product> Update(Product obj)
        {
            throw new NotImplementedException();
        }
    }
}