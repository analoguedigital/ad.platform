using LightMethods.Survey.Models.Services;
using System.Net;
using System.Web.Http;
using System.Linq;
using AutoMapper;
using LightMethods.Survey.Models.DTO;
using System;
using System.Data.Entity.Infrastructure;
using LightMethods.Survey.Models.Entities;

namespace WebApi.Controllers
{
    [RoutePrefix("api/vouchers")]
    [Authorize(Roles = "System administrator,Platform administrator")]
    public class VouchersController : BaseApiController
    {
        private SubscriptionService SubscriptionService { get; set; }

        public VouchersController()
        {
            this.SubscriptionService = new SubscriptionService(this.CurrentOrgUser, this.UnitOfWork);
        }

        public IHttpActionResult Get()
        {
            var vouchers = UnitOfWork.VouchersRepository.AllAsNoTracking;
            var result = vouchers.ToList().Select(v => Mapper.Map<VoucherDTO>(v));

            return Ok(result);
        }

        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var voucher = UnitOfWork.VouchersRepository.AllAsNoTracking
                .Where(v => v.Id == id)
                .SingleOrDefault();

            if (voucher == null)
                return NotFound();

            var result = Mapper.Map<VoucherDTO>(voucher);

            return Ok(result);
        }

        // POST api/vouchers
        [HttpPost]
        public IHttpActionResult Post([FromBody]VoucherDTO value)
        {
            var voucher = Mapper.Map<Voucher>(value);
            voucher.OrganisationId = Guid.Parse(value.Organisation.Id);
            voucher.Organisation = null;

            try
            {
                UnitOfWork.VouchersRepository.InsertOrUpdate(voucher);
                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/vouchers/5
        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Put(Guid id, [FromBody]VoucherDTO value)
        {
            var voucher = UnitOfWork.VouchersRepository.Find(id);
            if (voucher == null)
                return NotFound();

            voucher.Title = value.Title;
            voucher.Code = value.Code;
            voucher.Amount = value.Amount;
            voucher.OrganisationId = Guid.Parse(value.Organisation.Id);
            voucher.IsRedeemed = value.IsRedeemed;

            try
            {
                UnitOfWork.VouchersRepository.InsertOrUpdate(voucher);
                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/vouchers/5
        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            try
            {
                UnitOfWork.VouchersRepository.Delete(id);
                UnitOfWork.Save();

                return Ok();
            }
            catch(DbUpdateException dbEx)
            {
                return BadRequest(dbEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("redeem/{code}")]
        public IHttpActionResult Redeem(string code)
        {
            var result = this.SubscriptionService.RedeemCode(code);
            switch (result)
            {
                case SubscriptionService.RedeemCodeStatus.SubscriptionDisabled:
                    return Content(HttpStatusCode.Forbidden, "Subscriptions are disabled. Contact your administrator.");
                case SubscriptionService.RedeemCodeStatus.SubscriptionRateNotSet:
                    return Content(HttpStatusCode.Forbidden, "Subscription Rate is not set. Contact your administrator.");
                case SubscriptionService.RedeemCodeStatus.OK:
                    return Ok();
                default:
                    return NotFound();
            }   
        }
    }
}
