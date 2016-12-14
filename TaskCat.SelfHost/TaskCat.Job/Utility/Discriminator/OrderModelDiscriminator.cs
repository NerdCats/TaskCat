namespace TaskCat.Job.Utility.Discriminator
{
    using MongoDB.Bson.Serialization.Conventions;
    using System;
    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using Data.Model;

    public class OrderModelDiscriminator : IDiscriminatorConvention
    {
        private string orderType = "Type";    
        public string ElementName
        {
            get { return "_t"; }
        }

        public Type GetActualType(IBsonReader bsonReader, Type nominalType)
        {
            
            if (nominalType == typeof(OrderModel))
            {
                var ret = nominalType;

                var bookmark = bsonReader.GetBookmark();
                bsonReader.ReadStartDocument();
                if (bsonReader.FindElement(orderType))
                {
                    var value = bsonReader.ReadString();

                    ret = OrderTypeResolver.ResolveOrderType(value);

                    if (ret == null)
                        throw new Exception("Could not find type " + value);

                    if (!ret.IsSubclassOf(typeof(OrderModel)))
                        throw new Exception("Database type does not inherit from OrderModel.");
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
            if (nominalType != typeof(OrderModel))
                throw new Exception("Cannot use OrderDiscriminator for type " + nominalType);

            return actualType.AssemblyQualifiedName;
        }
    }
}