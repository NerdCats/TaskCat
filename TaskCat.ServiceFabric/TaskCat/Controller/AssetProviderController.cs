namespace TaskCat.Controller
{
    using Lib.Asset;
    using Model.Asset;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Lib.Constants;
    using Data.Model.Geocoding;

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

        /// <summary>
        /// Endpoint to search for assets available
        /// </summary>
        /// <param name="lat">
        /// latitude of designated location to find asset around
        /// </param>
        /// <param name="lon">
        /// longitude of designated location to find asset around
        /// </param>
        /// <param name="address">
        /// address of designated location to find asset around
        /// </param>
        /// <param name="radius">
        /// radius around designated location to find asset from
        /// </param>
        /// <param name="limit">
        /// limit the numbers of results, default is 10
        /// </param>
        /// <param name="strategy">
        /// search strategy to go for, default is QUICK
        /// </param>
        /// <returns>
        /// A List of available assets with their locations
        /// </returns>
        /// 

        [HttpGet]
        public async Task<IHttpActionResult> Search(
            double lat, double lon, string address, double? radius,
            int limit = AppConstants.DefaultAssetSearchLimit, SearchStrategy strategy = SearchStrategy.QUICK)
        {
            var request = new AssetSearchRequest()
            {
                Location = new DefaultAddress(address, new Data.Model.GeoJson.Point(new double[] { lon, lat }.ToList())),
                Limit = limit,
                Radius = radius,
                Strategy = strategy
            };

            var result = await provider.FindEligibleAssets(request);
            return Ok(result);
        }
    }
}
