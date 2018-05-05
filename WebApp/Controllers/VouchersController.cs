using LightMethods.Survey.Models.Services;
using System.Net;
using System.Web.Http;
using System.Linq;
using AutoMapper;
using LightMethods.Survey.Models.DTO;
using System;
using System.Data.Entity.Infrastructure;
using LightMethods.Survey.Models.Entities;
using System.Web.Hosting;
using System.Text;

namespace WebApi.Controllers
{
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

        // PUT api/vouchers/5
        [HttpPut]
        [Route("api/vouchers/{id:guid}")]
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

                        var email = new Email
                        {
                            To = this.CurrentOrgUser.Email,
                            Subject = $"Voucher Redeemed",
                            Content = GenerateVoucherEmail(voucher)
                        };

                        UnitOfWork.EmailsRepository.InsertOrUpdate(email);
                        UnitOfWork.Save();

                        return Ok();
                    }
                default:
                    return NotFound();
            }   
        }

        private string GenerateVoucherEmail(Voucher voucher)
        {
            var path = HostingEnvironment.MapPath("~/EmailTemplates/voucher-redeemed.html");
            var emailTemplate = System.IO.File.ReadAllText(path, Encoding.UTF8);

            var messageHeaderKey = "{{MESSAGE_HEADING}}";
            var messageBodyKey = "{{MESSAGE_BODY}}";

            var content = @"<p>You have redeemed your voucher code and are now subscribed.</p>
                            <p>This subscription has a fixed monthly quota. If you need more space to continue please purchase a paid plan or join an organization.</p>";

            emailTemplate = emailTemplate.Replace(messageHeaderKey, "Voucher Redeemed");
            emailTemplate = emailTemplate.Replace(messageBodyKey, content);

            return emailTemplate;
        }
    }
}
