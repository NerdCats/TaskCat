using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TaskCat.PartnerModels.Infini;
using TaskCat.PartnerServices.Infini;

namespace TaskCat.BackgroundJobService
{
    public class InfiniPollingService: HostedService, IDisposable
    {
        private ILogger _logger;
        private HttpClient _httpClient;
        private OrderService _orderService;
        private string _infiniToken;

        public InfiniPollingService(ILogger<InfiniPollingService> logger)
        {
            _logger = logger;
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

                foreach (var order in newOrders)
                {
                    await this._orderService.UpdateOrderStatus(this._infiniToken, order.id.ToString(), OrderStatusCode.Ready_To_Ship);
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
