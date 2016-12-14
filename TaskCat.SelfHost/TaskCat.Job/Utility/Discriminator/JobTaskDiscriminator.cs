namespace TaskCat.Job.Utility.Discriminator
{
    using MongoDB.Bson.Serialization.Conventions;
    using System;
    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using Data.Model;
    public class JobTaskDiscriminator : IDiscriminatorConvention 
    {
        public string ElementName
        {
            get { return "_t"; }
        }

        public Type GetActualType(IBsonReader bsonReader, Type nominalType)
        {
            if (nominalType == typeof(JobTask))
            {
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
            else
            {
                return nominalType;
            }
        }

        public BsonValue GetDiscriminator(Type nominalType, Type actualType)
        {
            if (nominalType != typeof(JobTask))
                throw new Exception("Cannot use JobTaskDiscriminator for type " + nominalType);

            return actualType.AssemblyQualifiedName;
        }
    }
}