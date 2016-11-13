namespace TaskCat.Lib.Utility.Odata
{
    using LinqToQuerystring;
    using Model.Pagination;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;

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
                if (item.Key.StartsWith("$"))
                {
                    if (string.IsNullOrEmpty(item.Value))
                    {
                        sb.Append(item.Key);
                    }
                    else
                    {
                        sb.Append(item.Key + "=" + item.Value);
                    }
                }
            }
            return sb.ToString();
        }

        public static async Task<object> ToOdataResponse<T>(this IQueryable<T> queryable, HttpRequestMessage request, string routeName)
        {
            var odataRequestModel = request.GetOdataRequestModel();
            if (odataRequestModel.CountOnly)
            {
                var queryTotal = queryable.LinqToQuerystring(queryString: odataRequestModel.OdataQueryString).Count();
                if (odataRequestModel.Envelope)
                {
                    var result = new PageEnvelope<T>(queryTotal, odataRequestModel.Page, odataRequestModel.PageSize, routeName, null, request);
                    return result;
                }
                return queryTotal;
            }
            else
            {
                if (odataRequestModel.ContainsSelect)
                {
                    var queryTotal = Task.Run(() => {
                        // delete the select first when Im counting
                        Regex selectParamRegex = new Regex(@"\$select=[a-zA-Z,\/]*");
                        var newQueryStringForCount = selectParamRegex.Replace(odataRequestModel.OdataQueryString, "");
                        var countResult = queryable.LinqToQuerystring(queryString: newQueryStringForCount).Count();
                        return countResult;
                    });

                    var newQueryString = odataRequestModel.OdataQueryString + $"$skip={odataRequestModel.Page * odataRequestModel.PageSize}$top={odataRequestModel.PageSize}";
                    var queryResult = Task.Run(() => {
                        var result = queryable.LinqToQuerystring(typeof(T), queryString: newQueryString) as IQueryable<Dictionary<string, object>>;
                        return result;
                    });

                    await Task.WhenAll(queryTotal, queryResult);

                    if (odataRequestModel.Envelope)
                    {
                        var result = new PageEnvelope<Dictionary<string, object>>(queryTotal.Result, odataRequestModel.Page, odataRequestModel.PageSize, routeName, queryResult.Result, request);
                        return result;
                    }

                    return queryResult;
                }
                else
                {
                    var queryTotal = Task.Run(() => queryable.LinqToQuerystring(queryString: odataRequestModel.OdataQueryString).Count());
                    var queryResult = Task.Run(
                        () => queryable.LinqToQuerystring(queryString: odataRequestModel.OdataQueryString)
                            .Skip(odataRequestModel.Page * odataRequestModel.PageSize)
                            .Take(odataRequestModel.PageSize)
                        );

                    await Task.WhenAll(queryTotal, queryResult);

                    if (odataRequestModel.Envelope)
                    {
                        var result = new PageEnvelope<T>(queryTotal.Result, odataRequestModel.Page, odataRequestModel.PageSize, routeName, queryResult.Result, request);
                        return result;
                    }

                    return queryResult;
                }
            }
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

            bool countOnly = false;
            if (requestMessage.Properties.ContainsKey(PagingQueryParameters.CountOnly))
            {
                countOnly = (bool)requestMessage.Properties[PagingQueryParameters.CountOnly];
            }

            bool containsSelect = false;
            if (requestMessage.Properties.ContainsKey("ContainsSelect"))
            {
                containsSelect = (bool)requestMessage.Properties["ContainsSelect"];
            }

            return new OdataRequestModel()
            {
                Envelope = (bool)requestMessage.Properties[PagingQueryParameters.Envelope],
                Page = (int)requestMessage.Properties[PagingQueryParameters.Page],
                PageSize = (int)requestMessage.Properties[PagingQueryParameters.PageSize],
                OdataQueryString = (string)requestMessage.Properties["OdataQueryString"],
                CountOnly = countOnly,
                ContainsSelect = containsSelect
            };
        }

    }
}