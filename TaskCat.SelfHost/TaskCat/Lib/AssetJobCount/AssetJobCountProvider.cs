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

            List<Job> jobsAssignedToAsset = await Collection.Find(x => x.State == JobState.IN_PROGRESS && x.Tasks.Any(y => y.AssetRef.Equals(AssetID))).ToListAsync();// all jobs assigned to this asset which are in progress

            if (jobsAssignedToAsset != null)
            {
                #region Pick_Up

                List<Job> jList_Pick_Up = jobsAssignedToAsset.Where(y => y.Tasks.Any(x => x.Type.Equals("PackagePickUp"))).ToList();// filtering all PackagePickUp Type jobs

                if (jList_Pick_Up != null)
                {
                    ajcm.Pick_Up.Assigned = jList_Pick_Up.Count;// total assigned PackagePickUp
                    ajcm.Pick_Up.Completed = jList_Pick_Up.Count(y => y.Tasks.Any(x => x.State == JobTaskState.COMPLETED));// total jobs COMPLETED
                    ajcm.Pick_Up.Failed = jList_Pick_Up.Count(y => y.Tasks.Any(x => x.State == JobTaskState.FAILED));// total jobs FAILED
                    ajcm.Pick_Up.Attempted = ajcm.Pick_Up.Completed + ajcm.Pick_Up.Failed; // sum of all jobs ATTEMPTED 
                }

                #endregion

                #region Delivery

                List<Job> jList_Delivery = jobsAssignedToAsset.Where(y => y.Tasks.Any(x => x.Type.Equals("Delivery"))).ToList();// filtering all Delivery Type jobs

                if (jList_Delivery != null)
                {
                    ajcm.Delivery.Assigned = jList_Delivery.Count;// total assigned Delivery
                    ajcm.Delivery.Completed = jList_Delivery.Count(y => y.Tasks.Any(x => x.State == JobTaskState.COMPLETED));
                    ajcm.Delivery.Failed = jList_Delivery.Count(y => y.Tasks.Any(x => x.State == JobTaskState.FAILED));
                    ajcm.Delivery.Attempted = ajcm.Delivery.Completed + ajcm.Delivery.Failed;
                }

                #endregion

                #region Exp_Delivery

                // we do not have concrete properties which will signify Exp_Delivery jobs

                #endregion 
            }

            if (jobsAssignedToAsset == null)
            {
                throw new EntityNotFoundException(typeof(AssetJobCountModel), AssetID);
            }
            return ajcm;
        }
    }
}