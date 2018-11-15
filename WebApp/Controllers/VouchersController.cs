using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Enums;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator,Platform administrator")]
    public class VouchersController : BaseApiController
    {
        #region Properties

        private const string CACHE_KEY = "VOUCHERS";

        private SubscriptionService SubscriptionService { get; set; }

        #endregion Properties

        #region C-tor

        public VouchersController()
        {
            SubscriptionService = new SubscriptionService(UnitOfWork);
        }

        #endregion C-tor

        #region CRUD

        // GET api/vouchers
        public IHttpActionResult Get()
        {
            var values = MemoryCacher.GetValue(CACHE_KEY);
            if (values == null)
            {
                var vouchers = UnitOfWork.VouchersRepository
                    .AllAsNoTracking
                    .ToList()
                    .Select(v => Mapper.Map<VoucherDTO>(v))
                    .ToList();

                MemoryCacher.Add(CACHE_KEY, vouchers, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(vouchers);
            }
            else
            {
                var result = (List<VoucherDTO>)values;
                return new CachedResult<List<VoucherDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/vouchers/{id}
        [Route("api/vouchers/{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var cacheKey = $"{CACHE_KEY}_{id}";
            var cachedValue = MemoryCacher.GetValue(cacheKey);

            if (cachedValue == null)
            {
                var voucher = UnitOfWork.VouchersRepository.Find(id);
                if (voucher == null)
                    return NotFound();

                var result = Mapper.Map<VoucherDTO>(voucher);
                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (VoucherDTO)cachedValue;
                return new CachedResult<VoucherDTO>(result, TimeSpan.FromMinutes(1), this);
            }
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

                MemoryCacher.Delete(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/vouchers/{id}
        [HttpPut]
        [Route("api/vouchers/{id:guid}")]
        public IHttpActionResult Put(Guid id, [FromBody]VoucherDTO value)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

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

                MemoryCacher.DeleteListAndItem(CACHE_KEY, id);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE api/vouchers/{id}
        [HttpDelete]
        [Route("api/vouchers/{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            try
            {
                UnitOfWork.VouchersRepository.Delete(id);
                UnitOfWork.Save();

                MemoryCacher.DeleteListAndItem(CACHE_KEY, id);

                return Ok();
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest(dbEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion CRUD

        #region Redeem Voucher

        // POST api/vouchers/redeem/{code}
        [HttpPost]
        [Route("api/vouchers/redeem/{code}")]
        [OverrideAuthorization]
        [Authorize(Roles = "Organisation user")]
        public IHttpActionResult Redeem(string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("voucher code is empty");

            if (CurrentOrgUser.AccountType != AccountType.MobileAccount)
                return BadRequest("vouchers are only available to mobile users");

            var result = SubscriptionService.RedeemCode(code, CurrentOrgUser);

            switch (result)
            {
                case RedeemCodeStatus.AlreadyRedeemed:
                    return BadRequest("this code has already been redeemed");
                case RedeemCodeStatus.SubscriptionDisabled:
                    return Content(HttpStatusCode.Forbidden, "subscriptions are disabled. contact your administrator.");
                case RedeemCodeStatus.SubscriptionRateNotSet:
                    return Content(HttpStatusCode.Forbidden, "subscription rate is not set. contact your administrator.");
                case RedeemCodeStatus.SubscriptionCountLessThanOne:
                    return Content(HttpStatusCode.Forbidden, "invalid subscription period. contact your administrator.");
                case RedeemCodeStatus.Error:
                    return BadRequest("an error has occured processing your code. try again or contact your administrator");
                case RedeemCodeStatus.NotFound:
                    return NotFound();
                case RedeemCodeStatus.OK:
                    {
                        NotifyUserAboutVoucherSubscription();
                        UnitOfWork.Save();

                        MemoryCacher.DeleteStartingWith(CACHE_KEY);

                        return Ok();
                    }
                default:
                    return NotFound();
            }
        }

        #endregion Redeem Voucher

        #region Helpers

        private void NotifyUserAboutVoucherSubscription()
        {
            var message = @"<p>You have redeemed your voucher code and are now subscribed.</p>
                            <p>This subscription has a fixed monthly quota. If you need more space to continue please purchase a paid plan or join an organization.</p>";

            var email = new Email
            {
                To = CurrentOrgUser.Email,
                Subject = $"Voucher Redeemed",
                Content = WebHelpers.GenerateEmailTemplate(message, "Voucher Redeemed")
            };

            UnitOfWork.EmailsRepository.InsertOrUpdate(email);
        }

        #endregion

    }
}
