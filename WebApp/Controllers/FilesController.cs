using LightMethods.Survey.Models.Services;
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

        [HttpPost]
        [Route("api/files")]
        [ResponseType(typeof(Guid))]
        public async Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
                return BadRequest("media type not supported");

            // EXPERIMENTAL.
            // validate the content-length against user's
            // available disk space. if the data has been
            // transferred over the network, this isn't necessary.
            // if we want to reject invalid file uploads, we should
            // do it on the client-side and reject big files before
            // sending them to server. otherwise we're wasting bandwidth.

            var statService = new StatisticsService(ServiceContext.UnitOfWork);
            var usedSpaced = statService.GetUsedSpace(ServiceContext.CurrentUser.Id);

            var subscriptionService = new SubscriptionService(ServiceContext.UnitOfWork);
            var quota = subscriptionService.GetMonthlyQuota(ServiceContext.CurrentUser.Id);

            if (quota.MaxDiskSpace.HasValue)
            {
                var availableDiskSpace = quota.MaxDiskSpace.Value - (int)usedSpaced.TotalSizeInKiloBytes;
                var contentSize = Request.Content.Headers.ContentLength / 1024;

                if (contentSize > availableDiskSpace)
                {
                    return BadRequest("You have exceeded your disk space allowance");
                }
            }


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
