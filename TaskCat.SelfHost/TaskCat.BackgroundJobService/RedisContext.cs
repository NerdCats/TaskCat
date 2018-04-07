using StackExchange.Redis;
using System;

namespace TaskCat.BackgroundJobService
{
    public class RedisContext
    {
        private string connectionString;
        public RedisContext(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
       
        public ConnectionMultiplexer Connection
        {
            get
            {
                return getLazyConnection().Value;
            }
        }

        private Lazy<ConnectionMultiplexer> getLazyConnection() 
            => new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(this.connectionString));
    }
}
