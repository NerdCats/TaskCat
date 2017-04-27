namespace TaskCat.Lib.Catalog
{
    using Data.Entity;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using System;
    using Common.Exceptions;
    using Common.Db;
    using Common.Domain;
    using MongoDB.Bson;
    using TaskCat.Lib.AssetJobCount;
    using TaskCat.Data.Model.Identity.Response;
    using System.Linq;
    using System.Linq.Expressions;
    using Data.Model;
    using System.Collections.Generic;

    public class AssetJobCountProvider : IAssetJobCountProvider
    {
        public IMongoCollection<Job> Collection { get; }

        public AssetJobCountProvider(IDbContext dbContext)
        {
            Collection = dbContext.Jobs;
        }

        public async Task<AssetJobCountModel> FindEligibleAssetJobCounts(string AssetID)
        {
            if (string.IsNullOrWhiteSpace(AssetID)) throw new ArgumentNullException(nameof(AssetID));

            AssetJobCountModel ajcm = new AssetJobCountModel();

            var jobsAssignedToAsset = await Collection.FindAsync(x => x.Tasks.Any(y => y.AssetRef == AssetID) && x.State == JobState.IN_PROGRESS);// all jobs assigned to this asset which are in progress

            IEnumerable<Job> jList = jobsAssignedToAsset.ToEnumerable();// make them IEnumerable to help linq

            #region Pick_Up

            IEnumerable<Job> jList_Pick_Up = jList.Where(y => y.Tasks.Any(x => x.Type.Equals("PackagePickUp")));// filtering all PackagePickUp Type jobs

            ajcm.Pick_Up.Assigned = jList_Pick_Up.Count();// total assigned PackagePickUp
            ajcm.Pick_Up.Completed = jList_Pick_Up.Count(y => y.Tasks.Any(x => x.State == JobTaskState.COMPLETED));// total jobs COMPLETED
            ajcm.Pick_Up.Failed = jList_Pick_Up.Count(y => y.Tasks.Any(x => x.State == JobTaskState.FAILED));// total jobs FAILED
            ajcm.Pick_Up.Attempted = ajcm.Pick_Up.Completed + ajcm.Pick_Up.Failed; // sum of all jobs ATTEMPTED

            #endregion

            #region Delivery

            IEnumerable<Job> jList_Delivery = jList.Where(y => y.Tasks.Any(x => x.Type.Equals("Delivery")));// filtering all Delivery Type jobs

            ajcm.Delivery.Assigned = jList_Delivery.Count();// total assigned Delivery
            ajcm.Delivery.Completed = jList_Delivery.Count(y => y.Tasks.Any(x => x.State == JobTaskState.COMPLETED));
            ajcm.Delivery.Failed = jList_Delivery.Count(y => y.Tasks.Any(x => x.State == JobTaskState.FAILED));
            ajcm.Delivery.Attempted = ajcm.Delivery.Completed + ajcm.Delivery.Failed;

            #endregion

            #region Exp_Delivery

            // we do not have concrete properties which will signify Exp_Delivery jobs

            #endregion

            if (jobsAssignedToAsset == null)
            {
                throw new EntityNotFoundException(typeof(AssetJobCountModel), AssetID);
            }
            return ajcm;
        }
    }
}