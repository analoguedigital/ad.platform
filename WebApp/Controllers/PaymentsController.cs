using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;

namespace WebApi.Controllers
{
    public class PaymentsController : BaseApiController
    {
        PaymentsRepository Payments { get { return UnitOfWork.PaymentsRepository; } }

        [DeflateCompression]
        [Route("api/payments")]
        [ResponseType(typeof(IEnumerable<PaymentRecordDTO>))]
        public IHttpActionResult Get()
        {
            var payments = this.Payments.AllAsNoTracking
                .Where(p => p.OrgUserId == this.CurrentOrgUser.Id)
                .OrderByDescending(p => p.DateCreated)
                .ToList();

            var result = payments.Select(p => Mapper.Map<PaymentRecordDTO>(p));

            return Ok(result);
        }
    }
}
