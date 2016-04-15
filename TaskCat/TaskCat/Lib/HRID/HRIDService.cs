namespace TaskCat.Lib.HRID
{
    using System;
    using System.Text;
    using Db;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Exceptions;

    public class HRIDService : IHRIDService
    {
        public HRIDService(IDbContext context)
        {
            _context = context;
        }

        private readonly object _padLock = new object();
        private IDbContext _context;

        private static char[] _base62chars =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

        private Random _random = new Random();
        private int length = 8;
        private int maxRetry = 25;

        public string NextId(string prefix)
        {
            lock (_padLock)
            {
                var dataTask = GenerateId();
                dataTask.Wait();
                return string.Concat(prefix, "#", dataTask.Result);             
            }
        }

        private async Task<string> GenerateId()
        {
            int count = 0;
            var sb = new StringBuilder(length);
            string generatedId;

            while (count < maxRetry)
            {
                for (int i = 0; i < length; i++)
                    sb.Append(_base62chars[_random.Next(36)]);

                generatedId = sb.ToString();
                var existingIdCount = await GetExistingIdCount(generatedId);
                if (existingIdCount == 0)
                {
                    await InsertNewHRID(generatedId);
                    return generatedId;
                }

                count++;
            }

            throw new ServerErrorException("Unique HRID generation failed");
        }

        public virtual async Task InsertNewHRID(string generatedId)
        {
            await _context.HRIDs.InsertOneAsync(new Data.Entity.HRIDEntity() { HRID = generatedId });
        }

        public virtual async Task<long> GetExistingIdCount(string generatedId)
        {
            return (await _context.HRIDs.Find(x => x.HRID == generatedId).CountAsync());
        }
    }
}