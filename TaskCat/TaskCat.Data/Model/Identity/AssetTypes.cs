namespace TaskCat.Data.Model.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    // INFO: Keeping the Supported AssetTypes to Capital just to make it readable over JSON cients
    public enum AssetTypes
    {        
        FETCHER,
        CNG_DRIVER,
        BIKE_MESSENGER
    }
}
