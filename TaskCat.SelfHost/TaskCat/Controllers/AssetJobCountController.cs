using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TaskCat.Data.Model.Geocoding;
using TaskCat.Lib.Asset;
using TaskCat.Lib.AssetJobCount;
using TaskCat.Lib.Constants;
using TaskCat.Model.Asset;

namespace TaskCat.Controllers
{
    /// <summary>
    /// Asset Providers are responsible for fetching eligible assets 
    /// for a certain job
    /// </summary>
    public class AssetJobCountController : ApiController
    {
        private IAssetJobCountProvider provider;
        public AssetJobCountController(IAssetJobCountProvider AssetJobCount)
        {
            provider = AssetJobCount;
        }

        /// <summary>
        /// Endpoint to search for assets available
        /// </summary>
        /// <param name="AssetID">
        /// AssetID which the asset app will provide
        /// </param>
       
        /// <returns>
        /// A json doc with job counts
        /// </returns>
        /// 

        [HttpGet]
        public async Task<IHttpActionResult> Search(
            string AssetID)
        {
            var result = await provider.FindEligibleAssetJobCounts(AssetID);
            return Ok(result);
        }
    }
}
