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

            List<Job> jobsAssignedToAsset = await Collection.Find
                (x => x.State == JobState.IN_PROGRESS
                    && x.Tasks.Any(
                                    y => !string.IsNullOrEmpty(y.AssetRef)
                                    && y.AssetRef.Equals(AssetID)
                                    && (y.Type == "PackagePickUp" || y.Type == "Delivery")
                                  )
                ).ToListAsync();// all jobs assigned to this asset which are in progress

            if (jobsAssignedToAsset != null)
            {
                #region Pick_Up

                var jList_Pick_Up_var = jobsAssignedToAsset.Where(y => y.Tasks.Exists(x => x.Type == "PackagePickUp" && x.AssetRef == AssetID));// filtering all PackagePickUp Type jobs

                List<Job> jList_Pick_Up = new List<Job>();

                if (jList_Pick_Up_var != null && jList_Pick_Up_var.Any())
                {
                    jList_Pick_Up = jList_Pick_Up_var.ToList();
                }

                if (jList_Pick_Up != null)
                {
                    ajcm.Pick_Up.Assigned = jList_Pick_Up.Count(y => y.Tasks.Exists(x => x.Type == "PackagePickUp"));// total assigned PackagePickUp
                    ajcm.Pick_Up.Completed = jList_Pick_Up.Count(y => y.Tasks.Exists(x => x.State == JobTaskState.COMPLETED && x.Type == "PackagePickUp"));// total jobs COMPLETED
                    ajcm.Pick_Up.Failed = jList_Pick_Up.Count(y => y.Tasks.Exists(x => x.State == JobTaskState.FAILED && x.Type == "PackagePickUp"));// total jobs FAILED
                    ajcm.Pick_Up.Covered = ajcm.Pick_Up.Completed + ajcm.Pick_Up.Failed > ajcm.Pick_Up.Assigned ? ((ajcm.Pick_Up.Completed > ajcm.Pick_Up.Failed) ? ajcm.Pick_Up.Completed : ajcm.Pick_Up.Failed) : ajcm.Pick_Up.Completed + ajcm.Pick_Up.Failed; // sum of all jobs ATTEMPTED 
                }

                #endregion

                #region Delivery

                var jList_Delivery_Var = jobsAssignedToAsset.Where(y => y.Tasks.Exists(x => x.Type == "Delivery" && x.AssetRef == AssetID));// filtering all Delivery Type jobs

                List<Job> jList_Delivery = new List<Job>();

                if (jList_Delivery_Var != null)
                {
                    jList_Delivery = jList_Delivery_Var.ToList();
                }

                if (jList_Delivery != null)
                {
                    ajcm.Delivery.Assigned = jList_Delivery.Count(y => y.Tasks.Exists(x => x.Type == "Delivery"));// total assigned Delivery
                    ajcm.Delivery.Completed = jList_Delivery.Count(y => y.Tasks.Exists(x => x.State == JobTaskState.COMPLETED && x.Type == "Delivery"));// total jobs COMPLETED
                    ajcm.Delivery.Failed = jList_Delivery.Count(y => y.Tasks.Exists(x => x.State == JobTaskState.FAILED && x.Type == "Delivery"));// total jobs FAILED
                    ajcm.Delivery.Covered = ajcm.Delivery.Completed + ajcm.Delivery.Failed > ajcm.Delivery.Assigned ? ((ajcm.Delivery.Completed > ajcm.Delivery.Failed) ? ajcm.Delivery.Completed : ajcm.Delivery.Failed) : ajcm.Delivery.Completed + ajcm.Delivery.Failed; // sum of all jobs ATTEMPTED 
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
