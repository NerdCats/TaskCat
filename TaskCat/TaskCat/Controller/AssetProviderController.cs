namespace TaskCat.Controller
{
    using Data.Model;
    using Lib.Asset;
    using Model.Asset;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Lib.Constants;

    /// <summary>
    /// Asset Providers are responsible for fetching eligible assets 
    /// for a certain job
    /// </summary>
    public class AssetProviderController : ApiController
    {
        private IAssetProvider provider;
        public AssetProviderController(IAssetProvider assetProvider)
        {
            provider = assetProvider;
        }

        [HttpGet]
        public async Task<IHttpActionResult> Search(
            double lat, double lon, string address, double? radius,
            int limit = AppConstants.DefaultAssetSearchLimit, SearchStrategy strategy = SearchStrategy.QUICK)
        {
            var request = new AssetSearchRequest()
            {
                Location = new Location()
                {
                    Address = address,
                    Point = new Data.Model.GeoJson.Point(new double[] { lon, lat }.ToList())
                },
                Limit = limit,
                Radius = radius,
                Strategy = strategy
            };

            var result = await provider.FindEligibleAssets(request);
            return Json(result);
        }
    }
}
