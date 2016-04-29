namespace TaskCat.Lib.Utility.Odata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class LinqToQuerystringExtensions
    {
        public static void VerifyQuery(this IEnumerable<KeyValuePair<string, string>> queryParams,
            IEnumerable<string> odataOptionsExceptions)
        {
            var exceptionParam = odataOptionsExceptions.Where(x => queryParams.Any(y => y.Key == x)).FirstOrDefault();
            if(exceptionParam != null)
                throw new NotSupportedException(String.Format("{0} is not supported in this endpoint", exceptionParam));
        }

        public static string GetOdataQuery(this IEnumerable<KeyValuePair<string, string>> queryParams,
            IEnumerable<string> otherParamsException = null)
        {
            var qParamDict = queryParams.ToDictionary(x => x.Key.ToLower(), x => x.Value);

            if (otherParamsException != null)
            {
                foreach (var param in otherParamsException)
                {
                    if (qParamDict.ContainsKey(param.ToLower()))
                        qParamDict.Remove(param);
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in qParamDict)
            {
                sb.Append(item.Key + "=" + item.Value);
            }
            return sb.ToString();
        }
    }
}