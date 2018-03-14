using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using TaskCat.PartnerModels.Infini;

namespace TaskCat.PartnerServices.Infini
{
    public class OrderService
    {
        private const string baseUri = "http://buyersclub.infinisystem.com";
        private HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<Order>> GetOrders(string token, string orderStatus = null)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = "api/orders/get-orders";

            if (!string.IsNullOrWhiteSpace(orderStatus))
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["order_status"] = orderStatus;
                uriBuilder.Query = query.ToString();
            }

            var request = new HttpRequestMessage()
            {
                RequestUri = uriBuilder.Uri,
                Method = HttpMethod.Get
            };

            request.Headers.Add("token", token);

            var response = await this._httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var returnVal = JsonConvert.DeserializeObject<List<Order>>(responseJson);

            return returnVal;
        }
    }
}
