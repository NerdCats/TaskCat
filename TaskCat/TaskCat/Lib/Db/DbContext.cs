namespace TaskCat.Lib.Db
{
    using Data.Entity;
    using MongoDB.Driver;
    using NLog;
    using System.Configuration;
    using Data.Entity.Identity;
    using MongoDB.Bson.Serialization.Conventions;
    using MongoDB.Bson;
    using AspNet.Identity.MongoDB;
    using Data.Entity.ShadowCat;

    public class DbContext : IDbContext
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private MongoClient mongoClient;
        private MongoClient shadowCatMongoClient;

        public IMongoDatabase Database { get; private set; }
        public IMongoDatabase ShadowCatDatabase { get; private set; }

        #region Auth
        
        private IMongoCollection<User> _users;
        public IMongoCollection<User> Users
        {
            get { return _users; }
        }

        private IMongoCollection<Asset> _assets;
        public IMongoCollection<Asset> Assets
        {
            get { return _assets; }
        }

        private IMongoCollection<Role> _roles;
        public IMongoCollection<Role> Roles
        {
            get { return _roles; }
        }

        private IMongoCollection<Client> _clients;
        public IMongoCollection<Client> Clients
        {
            get { return _clients; }
        }

        private IMongoCollection<RefreshToken> _refreshTokens;
        public IMongoCollection<RefreshToken> RefreshTokens
        {
            get { return _refreshTokens; }
        }

        #endregion

        #region JobsAndOrders

        private IMongoCollection<Job> _jobs;
        public IMongoCollection<Job> Jobs
        {
            get { return _jobs; }
        }

        private IMongoCollection<SupportedOrder> _supportedOrders;
        public IMongoCollection<SupportedOrder> SupportedOrders
        {
            get
            {
                return _supportedOrders;
            }
        }
        #endregion

        #region AssetTracking

        private IMongoCollection<AssetLocation> _assetLocations;
        public IMongoCollection<AssetLocation> AssetLocations
        {
            get
            {
                return _assetLocations;
            }
        }

        private IMongoCollection<HRIDEntity> _hrids;
        public IMongoCollection<HRIDEntity> HRIDs
        {
            get
            {
                return _hrids;
            }
        }

        #endregion

        public DbContext()
        {
            var pack = new ConventionPack()
            {
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("EnumConvensions", pack, t => true);

            InitiateDatabase();
            InitiateCollections();
            EnsureIndexes();
        }

        private void EnsureIndexes()
        {         
            IndexChecks.EnsureUniqueIndexOnUserName(_users);
            IndexChecks.EnsureUniqueIndexOnEmail(_users);
            IndexChecks.EnsureUniqueIndexOnRoleName(_roles);

            IndexFacade.EnsureUniqueIndexOnPhoneNumber(_users);
            IndexFacade.EnsureJobIndexes(_jobs);
            IndexFacade.EnsureHRIDIndex(_hrids);
        }

        private void InitiateCollections()
        {
            _users = Database.GetCollection<User>(CollectionNames.UsersCollectionName);
            _assets = Database.GetCollection<Asset>(CollectionNames.UsersCollectionName);
            _roles = Database.GetCollection<Role>(CollectionNames.RolesCollectionName);
            _clients = Database.GetCollection<Client>(CollectionNames.ClientsCollectionName);
            _refreshTokens = Database.GetCollection<RefreshToken>(CollectionNames.RefreshTokensCollectionName);
            _jobs = Database.GetCollection<Job>(CollectionNames.JobsCollectionName);
            _supportedOrders = Database.GetCollection<SupportedOrder>(CollectionNames.SupportedOrderCollectionName);
            _hrids = Database.GetCollection<HRIDEntity>(CollectionNames.HRIDCollectionName);

            _assetLocations = ShadowCatDatabase.GetCollection<AssetLocation>(ConfigurationManager.AppSettings["ShadowCat.LocationCacheCollectionName"]);          
        }

        private void InitiateDatabase()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Mongo.ConnectionString"].ConnectionString;
            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

            var TaskCatDBName = ConfigurationManager.AppSettings["TaskCat.DbName"];
            Database = mongoClient.GetDatabase(string.IsNullOrWhiteSpace(TaskCatDBName) ? DatabaseNames.TASKCAT_DB : TaskCatDBName);


            var shadowCatConnectionString = ConfigurationManager.ConnectionStrings["ShadowCat.ConnectionString"].ConnectionString;
            if (string.Equals(connectionString, shadowCatConnectionString))
                ShadowCatDatabase = Database;
            else
            {
                var shadowCatUrlBuilder = new MongoUrlBuilder(shadowCatConnectionString);
                shadowCatMongoClient = new MongoClient(shadowCatUrlBuilder.ToMongoUrl());
                if (shadowCatUrlBuilder.DatabaseName == "admin" || string.IsNullOrWhiteSpace(shadowCatUrlBuilder.DatabaseName))
                    //Load default shadowcat database name
                    ShadowCatDatabase = shadowCatMongoClient.GetDatabase(DatabaseNames.SHADOWCAT_DEFAULT_DB);
                else
                    ShadowCatDatabase = shadowCatMongoClient.GetDatabase(shadowCatUrlBuilder.DatabaseName);
            }
        }
    }
}