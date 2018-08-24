using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Hosting;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class VouchersController : BaseApiController
    {
        private SubscriptionService SubscriptionService { get; set; }

        public VouchersController()
        {
            this.SubscriptionService = new SubscriptionService(this.CurrentOrgUser, this.UnitOfWork);
        }

        // GET api/vouchers
        public IHttpActionResult Get()
        {
            var vouchers = UnitOfWork.VouchersRepository.AllAsNoTracking;
            var result = vouchers.ToList().Select(v => Mapper.Map<VoucherDTO>(v));

            return Ok(result);
        }

        // GET api/vouchers/{id}
        [Route("api/vouchers/{id:guid}")]
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

        // PUT api/vouchers/{id}
        [HttpPut]
        [Route("api/vouchers/{id:guid}")]
        public IHttpActionResult Put(Guid id, [FromBody]VoucherDTO value)
        {
            var voucher = UnitOfWork.VouchersRepository.Find(id);
            if (voucher == null)
                return NotFound();

            voucher.Title = value.Title;
            voucher.Code = value.Code;
            voucher.Period = value.Period;
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

        // DELETE api/vouchers/{id}
        [HttpDelete]
        [Route("api/vouchers/{id:guid}")]
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

        // POST api/vouchers/redeem/{code}
        [HttpPost]
        [Route("api/vouchers/redeem/{code}")]
        public IHttpActionResult Redeem(string code)
        {
            if (this.CurrentUser is SuperUser)
                return BadRequest("Vouchers are only available to mobile users");

            var result = this.SubscriptionService.RedeemCode(code);
            switch (result)
            {
                case SubscriptionService.RedeemCodeStatus.SubscriptionDisabled:
                    return Content(HttpStatusCode.Forbidden, "Subscriptions are disabled. Contact your administrator.");
                case SubscriptionService.RedeemCodeStatus.SubscriptionRateNotSet:
                    return Content(HttpStatusCode.Forbidden, "Subscription Rate is not set. Contact your administrator.");
                case SubscriptionService.RedeemCodeStatus.SubscriptionCountLessThanOne:
                    return Content(HttpStatusCode.Forbidden, "Subscription Count is zero! Contact your administrator.");
                case SubscriptionService.RedeemCodeStatus.OK:
                    {
                        var voucher = this.UnitOfWork.VouchersRepository.AllAsNoTracking.Where(x => x.Code == code).SingleOrDefault();


                        var message = @"<p>You have redeemed your voucher code and are now subscribed.</p>
                            <p>This subscription has a fixed monthly quota. If you need more space to continue please purchase a paid plan or join an organization.</p>";

                        var email = new Email
                        {
                            To = this.CurrentOrgUser.Email,
                            Subject = $"Voucher Redeemed",
                            Content = WebHelpers.GenerateEmailTemplate(message, "Voucher Redeemed")
                        };

                        UnitOfWork.EmailsRepository.InsertOrUpdate(email);
                        UnitOfWork.Save();

                        return Ok();
                    }
                default:
                    return NotFound();
            }   
        }

    }
}
