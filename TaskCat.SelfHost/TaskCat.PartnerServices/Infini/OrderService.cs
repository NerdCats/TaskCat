using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using TaskCat.PartnerModels.Infini;

namespace TaskCat.PartnerServices.Infini
{
    public class OrderService
    {
        private const string baseUri = "http://alladin.com.bd";
        private HttpClient _httpClient;

        // TODO: Make sure these are loaded from configuration/settings
        private const string username = "delivery@buyersclub.com";
        private const string password = "buyersclub@123$";

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> Login()
        {
            var uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = "/api/api-admin-login";

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["email"] = username;
            query["password"] = password;
            uriBuilder.Query = query.ToString();

            var request = new HttpRequestMessage()
            {
                RequestUri = uriBuilder.Uri,
                Method = HttpMethod.Post
            };

            var response = await this._httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            if (!response.Headers.TryGetValues("token", out IEnumerable<string> tokenVals))
                throw new InvalidOperationException("Failed to extract token from the headers");

            var token = tokenVals.First();
            return token;
        }

        public async Task<UpdateOrderResponse> UpdateOrderReferenceId(string token, string order_id, string taskCatJobId)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = "api/orders/update-api-shipping-id";

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["shipping_id"] = taskCatJobId ?? throw new ArgumentNullException(nameof(taskCatJobId));
            query["order_id"] = order_id ?? throw new ArgumentNullException(nameof(order_id));
            uriBuilder.Query = query.ToString();

            var request = new HttpRequestMessage()
            {
                RequestUri = uriBuilder.Uri,
                Method = HttpMethod.Post
            };

            request.Headers.Add("token", token);

            var response = await this._httpClient.SendAsync(request);
            //TODO: Do they actually send back an error code, not sure, need to handle this.
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var returnVal = JsonConvert.DeserializeObject<UpdateOrderResponse>(responseJson);

            return returnVal;
        }

        public async Task<UpdateOrderResponse> UpdateOrderStatus(string token, string order_id, OrderStatusCode order_status)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Invalid token provided", nameof(token));
            }

            if (string.IsNullOrWhiteSpace(order_id))
            {
                throw new ArgumentException("Invalid order_id provided", nameof(order_id));
            }

            if (order_status == OrderStatusCode.Undefined)
            {
                throw new ArgumentException("Invalid order_status provided", nameof(order_status));
            }

            var uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = "api/orders/api-update-order-status";

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["order_status"] = ((int)order_status).ToString();
            query["order_id"] = order_id;
            uriBuilder.Query = query.ToString();

            var request = new HttpRequestMessage()
            {
                RequestUri = uriBuilder.Uri,
                Method = HttpMethod.Post
            };

            request.Headers.Add("token", token);

            var response = await this._httpClient.SendAsync(request);
            //TODO: Do they actually send back an error code, not sure, need to handle this.
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var returnVal = JsonConvert.DeserializeObject<UpdateOrderResponse>(responseJson);

            return returnVal;
        }

        public async Task<IEnumerable<Order>> GetOrders(string token, OrderStatusCode orderStatus = OrderStatusCode.Undefined)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Invalid token provided", nameof(token));
            }

            var uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = "api/orders/get-orders";

            if (orderStatus != OrderStatusCode.Undefined)
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["order_status"] = ((int)orderStatus).ToString();
                uriBuilder.Query = query.ToString();
            }

            var request = new HttpRequestMessage()
            {
                RequestUri = uriBuilder.Uri,
                Method = HttpMethod.Post
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
