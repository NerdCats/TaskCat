﻿namespace TaskCat.Data.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Model;
    public interface IFetchable
    {
        Location FromLocation { get; set; }
        Location ToLocation { get; set; }
    }
}
