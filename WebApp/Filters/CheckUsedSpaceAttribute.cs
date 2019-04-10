using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApi.Filters
{
    public class CheckUsedSpaceAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var unitOfWork = ServiceContext.UnitOfWork;
            var currentUser = ServiceContext.CurrentUser;

            if (currentUser is OrgUser && ((OrgUser)currentUser).AccountType == AccountType.MobileAccount)
            {
                var statsService = new StatisticsService(unitOfWork);
                var usedSpace = statsService.GetUsedSpace(currentUser.Id);
                var fixedQuota = GetFixedMonthlyDiskSpace();

                if ((int)usedSpace.TotalSizeInKiloBytes >= fixedQuota)
                {
                    //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));
                }
            }

            base.OnActionExecuting(actionContext);
        }

        private int GetFixedMonthlyDiskSpace()
        {
            var quota = ConfigurationManager.AppSettings["FixedMonthlyDiskSpace"];
            if (!string.IsNullOrEmpty(quota))
                return int.Parse(quota);

            return 1024;  // default hard-coded value
        }

    }
}