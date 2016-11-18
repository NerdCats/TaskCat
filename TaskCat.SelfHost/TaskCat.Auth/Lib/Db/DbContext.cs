using System;
using MongoDB.Driver;
using TaskCat.Data.Entity.Identity;
using System.Configuration;
using AspNet.Identity.MongoDB;
using TaskCat.Data.Entity;
using TaskCat.Common.Db;

namespace TaskCat.Auth.Lib.Db
{
    public class DbContext : IDbContext
    {
        private MongoClient mongoClient;
        private MongoClient shadowCatMongoClient;

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
            IndexFacade.EnsureIndexesOnUserType(_users);
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

            var TaskCatDBName = ConfigurationManager.AppSettings["TaskCat.DbName"];
            Database = mongoClient.GetDatabase(string.IsNullOrWhiteSpace(TaskCatDBName) ? "taskcat" : TaskCatDBName);
        }
    }
}
