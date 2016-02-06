namespace TaskCat.Data.Model.Order
{
    using Data.Model;
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Entity;

    public class RideOrder : OrderModel
    {
        public RideOrder(string name = null) : base(name, "Ride")
        {

        }

        public Location From { get; set; }
        public Location To { get; set; }

        /// <summary>
        /// Basically a list of vehicles users want to avail and all of these options 
        /// are basically
        /// a or relation
        /// </summary>
        /// 
        // FIXME: No default vehicle preference
        public List<string> VehiclePreference { get; set; }

        //FIXME: Im still not sure whether Id want the system to have
        //capability to allow users to interact with the app to select vehicles around them or not
        [BsonIgnoreIfNull]
        public Asset ProposedRide { get; set; }
    }

    //FIXME: This really shouldnt be done this way man, 
    //we really need to tie something cool here for all vehicles
    public class VehiclePreference
    {
        public const string CNG = "CNG";
        public const string SEDAN = "SEDAN";
    }
}
