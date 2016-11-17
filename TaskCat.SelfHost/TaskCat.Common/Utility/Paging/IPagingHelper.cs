using System.Collections.Generic;

namespace TaskCat.Common.Utility.Paging
{
    public interface IPagingHelper
    {
        string GeneratePageUrl(string route, long page, long pageSize, IDictionary<string, string> otherParams = null);
    }
}