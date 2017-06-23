using AutoMapper;
using LightMethods.Survey.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class PaymentsController : BaseApiController
    {
        PaymentsRepository Payments { get { return UnitOfWork.PaymentsRepository; } }

        [Route("api/payments/{userId:guid}")]
        [ResponseType(typeof(IEnumerable<PaymentRecordDTO>))]
        public IHttpActionResult Get(Guid userId)
        {
            var payments = this.Payments.AllAsNoTracking
                .Where(p => p.OrgUserId == userId)
                .ToList();
            var result = payments.Select(p => Mapper.Map<PaymentRecordDTO>(p));

            return Ok(result);
        }
    }
}
