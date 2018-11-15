using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Organisation user")]
    public class PaymentsController : BaseApiController
    {

        // GET api/payments
        [DeflateCompression]
        [Route("api/payments")]
        [ResponseType(typeof(IEnumerable<PaymentRecordDTO>))]
        public IHttpActionResult Get()
        {
            // not necessary to check the OrgAdmin role,
            // only mobile accounts have access anyway.
            //var isOrgAdmin = await ServiceContext.UserManager.IsInRoleAsync(CurrentOrgUser.Id, "Organisation administrator");

            if (CurrentOrgUser.AccountType != AccountType.MobileAccount)
                return BadRequest("payments are only available to mobile users");

            var payments = UnitOfWork.PaymentsRepository
                .AllAsNoTracking
                .Where(p => p.OrgUserId == CurrentOrgUser.Id)
                .OrderByDescending(p => p.DateCreated)
                .ToList()
                .Select(p => Mapper.Map<PaymentRecordDTO>(p))
                .ToList();

            return Ok(payments);
        }
    }
}
