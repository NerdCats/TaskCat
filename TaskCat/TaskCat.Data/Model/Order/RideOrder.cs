using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskCat.Data.Model.Order
{
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
        public List<string> VehiclePreference { get; set; }

    }

    //FIXME: This really shouldnt be done this way man, 
    //we really need to tie something cool here for all vehicles
    public class VehiclePreference
    {
        public const string CNG = "CNG";
        public const string SEDAN = "SEDAN";
    }
}
