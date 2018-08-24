using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebApi.Controllers
{
    public class FilesController : ApiController
    {
        HttpContext Context
        {
            get { return HttpContext.Current; }
        }

        // GET api/files
        [ResponseType(typeof(Guid))]
        [HttpPost]
        [Route("api/files")]
        public async Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
                return BadRequest("Unsupported media type");

            var tempId = Guid.NewGuid();
            var workingFolder = HttpContext.Current.Server.MapPath("~/Uploads/" + tempId);
            Directory.CreateDirectory(workingFolder);

            try
            {
                var provider = new CustomMultipartFormDataStreamProvider(workingFolder);

                await Request.Content.ReadAsMultipartAsync(provider);

                var files = provider.FileData
                    .Select(file => new FileInfo(file.LocalFileName))
                    .ToList();

                return Ok(tempId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }
    }
}
