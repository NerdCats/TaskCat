namespace TaskCat.Account.Propagation
{
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System;
    using Common.Db;
    using Data.Model.Identity.Response;
    using MongoDB.Driver;
    using Data.Entity;
    using Data.Model.Identity;
    using Data.Entity.Identity;
    using System.Linq;

    /// <summary>
    /// Update service for changes in Account and profiles.
    /// </summary>
    public class AccountUpdatePropagationService
    {
        private IDbContext dbContext;
        private IObservable<User> userUpdateSource;

        /// <summary>
        /// Instantiates a AccountUpdatePropagationService.
        /// </summary>
        /// <param name="dbcontext">IDbContext for database context.</param>
        /// <param name="userUpdateSource">Observable source for User updates.</param>
        public AccountUpdatePropagationService(
            IDbContext dbcontext,
            IObservable<User> userUpdateSource)
        {
            if (dbcontext == null)
                throw new ArgumentNullException(nameof(dbcontext));
            if (userUpdateSource == null)
                throw new ArgumentNullException(nameof(userUpdateSource));

            this.dbContext = dbcontext;
            this.userUpdateSource = userUpdateSource;

            this.userUpdateSource
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Subscribe(OnUserUpdate, OnUserUpdateError);
        }

        private void OnProfileUpdate(UserModel model)
        {
            UpdateComments(model);
        }

        private void OnUserUpdate(User user)
        {
            UpdateJobs(user);
            UpdateComments(user.ToModel());
        }

        private void UpdateJobs(User user)
        {
            /*
             * TODO: We don't really have any error handlers here, and this would essentially be very hard to handle errors
             * with this way. Actor model might be a better option in this case someday. Plus, we are not even checking whether 
             * the update was successful or not. We might need to check that too.
             */
            if (user.Type != IdentityTypes.USER && user.Type != IdentityTypes.ENTERPRISE)
            {
                var updateDef = Builders<Job>.Update.Set(x => x.Assets[user.Id], new AssetModel(user as Asset));
                var searchFilter = Builders<Job>.Filter.Exists(x => x.Assets[user.Id], true);
                var propagationResult = dbContext.Jobs.UpdateMany(searchFilter, updateDef);
            }
            else if (user.Roles.Any(x => x == "Administrator" || x == "BackOfficeAdmin"))
            {
                var userModel = new UserModel(user);
                var updateDef = Builders<Job>.Update.Set(x => x.JobServedBy, userModel);
                var searchFilter = Builders<Job>.Filter.Where(x => x.JobServedBy.UserId == user.Id);
                var propagationResult = dbContext.Jobs.UpdateMany(searchFilter, updateDef);
            }
            else if (user.Type == IdentityTypes.USER && user.Type == IdentityTypes.ENTERPRISE)
            {
                var userModel = user.Type == IdentityTypes.USER ? new UserModel(user) : new EnterpriseUserModel(user as EnterpriseUser);
                var updateDef = Builders<Job>.Update.Set(x => x.User, userModel);
                var searchFilter = Builders<Job>.Filter.Where(x => x.User.UserId == user.Id);
                var propagationResult = dbContext.Jobs.UpdateMany(searchFilter, updateDef);
            }
        }

        private void UpdateComments(UserModel model)
        {
            var updateDef = Builders<Comment>.Update
                .Set(x => x.User, new ReferenceUser(model));

            dbContext.Comments.UpdateMany(
                Builders<Comment>.Filter.Where(x => x.User.Id == model.UserId),
                updateDef);
        }

        private void OnUserUpdateError(Exception ex)
        {
            Console.WriteLine(ex);
            // TODO: Log the exception here and we might need to redo things here
        }

        private void OnProfileUpdateError(Exception ex)
        {
            Console.WriteLine(ex);
            // TODO: Log the exception here and we might need to redo things here
        }
    }
}
