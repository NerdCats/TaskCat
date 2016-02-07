namespace TaskCat.Lib.Db
{
    using Data.Entity;
    using MongoDB.Driver;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using Data.Entity.Identity;
    using MongoDB.Bson.Serialization.Conventions;
    using MongoDB.Bson;
    using AspNet.Identity.MongoDB;

    public class DbContext : IDbContext
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private MongoClient mongoClient;


        public IMongoDatabase Database { get; private set; }

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


        private IMongoCollection<Job> _jobs;
        public IMongoCollection<Job> Jobs
        {
            get { return _jobs; }
        }

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
        }

        private void InitiateCollections()
        {
            _users = Database.GetCollection<User>(CollectionNames.UsersCollectionName);
            _assets = Database.GetCollection<Asset>(CollectionNames.UsersCollectionName);
            _roles = Database.GetCollection<Role>(CollectionNames.RolesCollectionName);
            _clients = Database.GetCollection<Client>(CollectionNames.ClientsCollectionName);
            _refreshTokens = Database.GetCollection<RefreshToken>(CollectionNames.RefreshTokensCollectionName);

            _jobs = Database.GetCollection<Job>(CollectionNames.JobsCollectionName);

        }

        private void InitiateDatabase()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Mongo.ConnectionString"].ConnectionString;
            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            Database = mongoClient.GetDatabase(mongoUrlBuilder.DatabaseName);
        }
    }
}