namespace TaskCat.Data.Model.Identity.Response
{
    using Entity.ShadowCat;
    using Entity.Identity;

    public class AssetJobCountModel
    {
        public AssetJobCountModel()
        {

        }
        public AssetJobCountModelBase Pick_Up { get; set; }
        public AssetJobCountModelBase Delivery { get; set; }
        public AssetJobCountModelBase Exp_Delivery { get; set; }
    }

    public class AssetJobCountModelBase
    {
        public AssetJobCountModelBase()
        {
            Assigned = 0;
            Completed = 0;
            Failed = 0;
            Attempted = 0;
        }

        public long Assigned { get; set; } // total jobs assigned to the asset
        public long Completed { get; set; }
        public long Failed { get; set; }
        public long Attempted { get; set; } // sum of completed and failed
    }
}
