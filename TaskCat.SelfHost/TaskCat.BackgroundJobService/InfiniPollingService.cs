using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TaskCat.Common.Settings;
using TaskCat.Data.Model.Geocoding;
using TaskCat.Data.Model.Inventory;
using TaskCat.Data.Model.Order.Delivery;
using TaskCat.Data.Model.Person;
using TaskCat.PartnerModels.Infini;
using TaskCat.PartnerServices.Infini;
using TaskCat.Payment.Core.Manual;

namespace TaskCat.BackgroundJobService
{
    public class InfiniPollingService: HostedService, IDisposable
    {
        private const string DefaultAddressConfiguration = "DefaultFromAddress";
        private readonly ILogger _logger;
        private readonly IConfiguration configuration;
        private HttpClient _httpClient;
        private OrderService _orderService;
        private string _infiniToken;

        public InfiniPollingService(
            ILogger<InfiniPollingService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
            _httpClient = new HttpClient();
            _orderService = new OrderService(_httpClient);
        }

        private async Task DoWork()
        {
            _logger.LogInformation("Background task is working.");

            try
            {
                _logger.LogInformation("Fetching token");
                this._infiniToken = await this._orderService.Login();
                
                _logger.LogInformation("Fetching new orders");
                var newOrders = await this._orderService.GetOrders(this._infiniToken, OrderStatusCode.Ready_To_Ship);

                // TODO
                /* 1. Set a customer ID for infini in taskcat
                 * 4. Find a way so TaskCat can communicate everytime these jobs are updated. 
                 * 5. When any of these jobs are updated TaskCat will notify some message channel .
                 * 6. Add a new code block here that listens to that channel and updates the job using something like the following/
                 * */

                var defaultAddressSettings =
                    this.configuration.GetSection(DefaultAddressConfiguration).Get<ProprietorSettings>();

                // Sample update code, possibly useless here, we shouldn't use it anyway.
                foreach (var order in newOrders)
                {
                    var taskcatOrder = order.ToClassifiedDeliveryOrder(defaultAddressSettings);
                    //await this._orderService.UpdateOrderStatus(this._infiniToken, order.id.ToString(), OrderStatusCode.Ready_To_Ship);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error encountered");
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Infini Background Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                await DoWork();
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
}
