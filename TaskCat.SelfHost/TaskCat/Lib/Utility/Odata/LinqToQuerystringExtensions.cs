namespace TaskCat.Lib.Utility.Odata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;

    internal static class LinqToQuerystringExtensions
    {
        public static void VerifyQuery(this IEnumerable<KeyValuePair<string, string>> queryParams,
            IEnumerable<string> odataOptionsExceptions)
        {
            var exceptionParam = odataOptionsExceptions.Where(x => queryParams.Any(y => y.Key == x)).FirstOrDefault();
            if (exceptionParam != null)
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
                        qParamDict.Remove(param.ToLower());
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in qParamDict)
            {
                sb.Append(item.Key + "=" + item.Value);
            }
            return sb.ToString();
        }

        public static string GetOdataQueryString(this HttpRequestMessage requestMessage, List<string> otherParamsException)
        {
            if (requestMessage == null)
                throw new ArgumentNullException(nameof(requestMessage));

            var queryParams = requestMessage.GetQueryNameValuePairs();
            queryParams.VerifyQuery(new List<string>() {
                    OdataOptionExceptions.InlineCount,
                    OdataOptionExceptions.Skip,
                    OdataOptionExceptions.Top
                });

            return queryParams.GetOdataQuery(otherParamsException);
        }

        public static OdataRequestModel GetOdataRequestModel(this HttpRequestMessage requestMessage)
        {
            if (requestMessage == null)
                throw new ArgumentNullException(nameof(requestMessage));

            if (!requestMessage.Properties.ContainsKey("OdataQueryString"))
                throw new ArgumentException("Request doesn't contain OdataQueryString property");
            if (!requestMessage.Properties.ContainsKey(PagingQueryParameters.Page))
                throw new ArgumentException($"Request doesn't contain {PagingQueryParameters.Page} property");
            if (!requestMessage.Properties.ContainsKey(PagingQueryParameters.PageSize))
                throw new ArgumentException($"Request doesn't contain {PagingQueryParameters.PageSize} property");
            if (!requestMessage.Properties.ContainsKey(PagingQueryParameters.Envelope))
                throw new ArgumentException($"Request doesn't contain {PagingQueryParameters.Envelope} property");

            return new OdataRequestModel()
            {
                Envelope = (bool)requestMessage.Properties[PagingQueryParameters.Envelope],
                Page = (int)requestMessage.Properties[PagingQueryParameters.Page],
                PageSize = (int)requestMessage.Properties[PagingQueryParameters.PageSize],
                OdataQueryString = (string)requestMessage.Properties["OdataQueryString"]
            };
        }
    }
}