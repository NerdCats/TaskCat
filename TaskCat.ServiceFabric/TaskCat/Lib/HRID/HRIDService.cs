namespace TaskCat.Lib.HRID
{
    using System;
    using System.Text;
    using Db;
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
                var data = GenerateId();
                return string.Concat(prefix, "-", data);
            }
        }

        private string GenerateId()
        {
            int count = 0;
            var sb = new StringBuilder(length);
            string generatedId;

            while (count < maxRetry)
            {
                for (int i = 0; i < length; i++)
                    sb.Append(_base62chars[_random.Next(36)]);

                generatedId = sb.ToString();
                var existingIdCount = GetExistingIdCount(generatedId);
                if (existingIdCount == 0)
                {
                    InsertNewHRID(generatedId);
                    return generatedId;
                }

                count++;
            }

            throw new ServerErrorException("Unique HRID generation failed");
        }

        public virtual void InsertNewHRID(string generatedId)
        {
            _context.HRIDs.InsertOne(new Data.Entity.HRIDEntity() { HRID = generatedId });
        }

        public virtual long GetExistingIdCount(string generatedId)
        {
            return _context.HRIDs.Count(x => x.HRID == generatedId);
        }
    }
}