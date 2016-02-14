namespace TaskCat.Data.Lib.Utility.Converter
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TaskCat.Data.Model;
    using Constants;
    using Model.JobTasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class JobTaskConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JobTask);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.Null)
                return null;

            var obj = JObject.Load(reader);
            JobTask jobTask;

            var type = obj["Type"];
            if (type == null)
            {
                throw new InvalidOperationException("JobTask Type not provided");
            }

            string jobTaskType = type.Value<string>();

            switch (jobTaskType)
            {
                case JobTaskTypes.FETCH_RIDE:
                    jobTask = new FetchRideTask();
                    break;
                case JobTaskTypes.RIDE_PICKUP:
                    jobTask = new RidePickUpTask();
                    break;
                default:
                    throw new NotSupportedException("JobTask Type invalid/not supported yet");
            }

            serializer.Populate(obj.CreateReader(), jobTask);
            return jobTask;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}