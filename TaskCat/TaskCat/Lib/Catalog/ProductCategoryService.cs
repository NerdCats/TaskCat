namespace TaskCat.Lib.Catalog
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Data.Entity;
    using Db;
    using Exceptions;

    public class ProductCategoryService : IProductCategoryService
    {
        private IDbContext dbContext;

        public IMongoCollection<ProductCategory> Collection { get; set; }

        public ProductCategoryService(IDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.Collection = dbContext.ProductCategories;
        }

        public async Task<ProductCategory> Delete(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            var result = await Collection.FindOneAndDeleteAsync(x => x.Id == id);
            var pullFilter = Builders<Product>.Update.PullFilter(x => x.ProductCategories, y => y.Id == id);
            var productUpdateResult = await dbContext.Products.UpdateManyAsync(x => x.ProductCategories != null, pullFilter);

            // We have to kind of do something here to be hoenst. As we are moving code off Products here, 
            // we need to make sure this goes well 

            if (result == null)
                throw new EntityDeleteException(typeof(ProductCategory), id);
            return result;
        }

        public async Task<ProductCategory> Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            var result = (await Collection.Find(x => x.Id == id).ToListAsync()).FirstOrDefault();
            if (result == null)
            {
                throw new EntityNotFoundException(typeof(ProductCategory), id);
            }
            return result;
        }

        public async Task<ProductCategory> Insert(ProductCategory obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            obj.CreateTime = DateTime.UtcNow; 
            await Collection.InsertOneAsync(obj);
            return obj;
        }

        public async Task<ProductCategory> Update(ProductCategory obj)
        {
            if (String.IsNullOrWhiteSpace(obj.Id)) throw new ArgumentNullException(nameof(obj.Id));

            obj.LastModified = DateTime.UtcNow;
            var result = await Collection.FindOneAndReplaceAsync(x => x.Id == obj.Id , obj);
            if (result == null)
                throw new EntityUpdateException(typeof(ProductCategory), obj.Id);
            return result;
        }
    }
}