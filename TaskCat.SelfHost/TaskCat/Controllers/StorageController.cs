using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TaskCat.Lib.Storage;

namespace TaskCat.Controllers
{
    /// <summary>
    /// StorageController is the official controller to upload image and 
    /// other files to TaskCat storage service for various purpose
    /// </summary>
    [RoutePrefix("api/Storage")]
    public class StorageController : ApiController
    {
        private IStorageRepository _repository;

        /// <summary>
        /// StorageController Constructor
        /// </summary>
        /// <param name="repository"></param>
        public StorageController(IStorageRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Post Endpoint for uploading images
        /// Its a multi-part form data request.
        /// Post the image in path "image"
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Authorize(Roles = "User, Asset")]
        [Route("image")]
        public async Task<IHttpActionResult> Image()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                var result = await _repository.UploadImage(Request.Content);

                if (result != null)
                {
                    return Ok(result);
                }

                return BadRequest();
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Deletes a file from Storage Service
        /// </summary>
        /// <param name="fileName">
        /// desired file name to be deleted
        /// </param>
        /// <returns>
        /// File Deletion Status as a FileDeleteModel
        /// </returns>
        [HttpDelete]
        [AllowAnonymous]
        [Authorize(Roles = "User, Asset")]
        [Route("image")]
        public async Task<IHttpActionResult> DeleteImage(string fileName)
        {
            var result = await _repository.DeleteImage(fileName);
            switch(result.Status)
            {
                case Model.Storage.DeletionStatus.FAILED:
                    return Content(HttpStatusCode.InternalServerError, result);
                case Model.Storage.DeletionStatus.FILENOTFOUND:
                    return Content(HttpStatusCode.BadRequest, result);
                case Model.Storage.DeletionStatus.SUCCESSFUL:
                    return Ok(result);
                default:
                    return Content(HttpStatusCode.NotImplemented, "Deletion Status Not Supported yet");
            }
        }
    }
}
