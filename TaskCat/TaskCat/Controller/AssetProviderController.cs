namespace TaskCat.Controller
{
    using Data.Model;
    using Lib.Asset;
    using Model.Asset;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

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
        public async Task<IHttpActionResult> Search()
        {
            var result = await provider.FindEligibleAssets(new AssetSearchRequest()
            {
                Location = new Location()
                {
                    Point = new Data.Model.GeoJson.Point(new double[] { 90.4075033, 23.796605 }.ToList())
                }
            });
            return Json(result);
        }
    }
}
