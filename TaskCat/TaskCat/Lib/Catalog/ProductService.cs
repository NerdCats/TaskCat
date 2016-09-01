namespace TaskCat.Lib.Catalog
{
    using Data.Entity;
    using Exceptions;
    using Domain;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Db;
    using System;

    public class ProductService : IRepository<Product>
    {
        public IMongoCollection<Product> Collection { get; }

        public ProductService(IDbContext dbContext)
        {
            Collection = dbContext.Products;
        }

        public async Task<Product> Delete(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            var result = await Collection.FindOneAndDeleteAsync(x => x.Id == id);
            if (result == null)
                throw new EntityDeleteException(typeof(Product), id);
            return result;
        }

        public async Task<Product> Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            var result = await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (result == null)
            {
                throw new EntityNotFoundException(typeof(Product), id);
            }
            return result;
        }

        public async Task<Product> Insert(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            await Collection.InsertOneAsync(product);
            return product;
        }

        public async Task<Product> Update(Product obj)
        {
            if (String.IsNullOrWhiteSpace(obj.Id)) throw new ArgumentNullException(nameof(obj.Id));
            var result = await Collection.FindOneAndReplaceAsync(x => x.Id == obj.Id, obj);
            if (result == null)
                throw new EntityUpdateException(typeof(Product), obj.Id);
            return result;
        }
    }
}