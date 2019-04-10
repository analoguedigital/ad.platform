using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class StatsController : BaseApiController
    {

        private StatisticsService StatisticsService { get; set; }

        public StatsController()
        {
            StatisticsService = new StatisticsService(UnitOfWork);
        }

        [Authorize(Roles = "System administrator,Platform administrator")]
        [Route("api/stats/platform")]
        public IHttpActionResult GetPlatformStats()
        {
            var result = StatisticsService.GetPlatformStats();
            return Ok(result);
        }

        [Authorize(Roles = "System administrator,Platform administrator,Organisation administrator")]
        [Route("api/stats/user/{id:guid}")]
        public IHttpActionResult GetUserStatistics(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("User Id is empty");

            var stats = StatisticsService.GetUserStats(id);
            return Ok(stats);
        }

        //[Route("api/stats")]
        //public IHttpActionResult GetStatistics()
        //{
        //    var stats = StatisticsService.GetUserStats(CurrentOrgUser.Id);
        //    return Ok(stats);
        //}

        //[Route("api/stats/used-space")]
        //public IHttpActionResult GetUsedSpace()
        //{
        //    var usedSpace = StatisticsService.GetUsedSpace(CurrentOrgUser.Id);
        //    return Ok(usedSpace);
        //}

        //[Route("api/stats/total-used-space")]
        //public IHttpActionResult GetTotalUsedSpace()
        //{
        //    var totalUsedSpace = StatisticsService.GetTotalUsedSpace(CurrentOrgUser.Id);
        //    return Ok(totalUsedSpace);
        //}

    }
}
