namespace TaskCat.Lib.Catalog
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Data.Entity;
    using Db;
    using Domain;
    using Common.Exceptions;

    public class ProductCategoryService : IRepository<ProductCategory>
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

            var updateFilter = Builders<Product>.Update.PullFilter(p=>p.Categories, f => f.Id == id);
            var updateResult = await dbContext.Products.UpdateManyAsync(x => true, updateFilter);

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

            var productUpdateFilter = Builders<Product>.Filter.ElemMatch(x => x.Categories, x => x.Id == obj.Id);
            
            var productUpdate = Builders<Product>.Update.Set($"{nameof(Product.Categories)}.$", obj);
            await dbContext.Products.UpdateManyAsync(productUpdateFilter, productUpdate);

            if (result == null)
                throw new EntityUpdateException(typeof(ProductCategory), obj.Id);
            return result;
        }
    }
}