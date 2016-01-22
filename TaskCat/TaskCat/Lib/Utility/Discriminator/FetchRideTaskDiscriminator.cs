using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using TaskCat.Model.JobTasks;
using TaskCat.Data.Entity.Assets;
using TaskCat.Data.Model;

namespace TaskCat.Lib.Utility.Discriminator
{
    public class FetchRideTaskDiscriminator : IDiscriminatorConvention
    {
        public string ElementName
        {
            get { return "_t"; }
        }

        public Type GetActualType(IBsonReader bsonReader, Type nominalType)
        {
            if (nominalType != typeof(FetchRideTask<Ride>))
                throw new Exception("Cannot use FetchRideTaskDiscriminator for type " + nominalType);

            var ret = nominalType;

            var bookmark = bsonReader.GetBookmark();
            bsonReader.ReadStartDocument();
            if (bsonReader.FindElement(ElementName))
            {
                var value = bsonReader.ReadString();

                ret = Type.GetType(value);

                if (ret == null)
                    throw new Exception("Could not find type " + value);

                if (!ret.IsSubclassOf(typeof(JobTask)))
                    throw new Exception("Database type does not inherit from JobTask.");
            }

            bsonReader.ReturnToBookmark(bookmark);

            return ret;
        }

        public BsonValue GetDiscriminator(Type nominalType, Type actualType)
        {
            if (nominalType != typeof(FetchRideTask<>))
                throw new Exception("Cannot use JobTaskDiscriminator for type " + nominalType);

            return actualType.FullName;
        }
    }
}