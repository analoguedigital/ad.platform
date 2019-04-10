using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [RoutePrefix("api/subscriptionplans")]
    [Authorize(Roles = "System administrator,Platform administrator")]
    public class SubscriptionPlansController : BaseApiController
    {

        private const string CACHE_KEY = "SUBSCRIPTION_PLANS";

        // GET api/subscriptionPlans
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Platform administrator,Organisation user,Restricted user")]
        public IHttpActionResult Get()
        {
            var cacheEntry = MemoryCacher.GetValue(CACHE_KEY);
            if (cacheEntry == null)
            {
                var plans = UnitOfWork.SubscriptionPlansRepository
                    .AllAsNoTracking
                    .ToList();

                var result = plans.Select(sp => Mapper.Map<SubscriptionPlanDTO>(sp)).ToList();
                MemoryCacher.Add(CACHE_KEY, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (List<SubscriptionPlanDTO>)cacheEntry;
                return new CachedResult<List<SubscriptionPlanDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/subscriptionPlans/{id}
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var cacheKey = $"{CACHE_KEY}_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var plan = UnitOfWork.SubscriptionPlansRepository.Find(id);
                if (plan == null)
                    return NotFound();

                var result = Mapper.Map<SubscriptionPlanDTO>(plan);
                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (SubscriptionPlanDTO)cacheEntry;
                return new CachedResult<SubscriptionPlanDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/subscriptionPlans
        [HttpPost]
        public IHttpActionResult Post([FromBody]SubscriptionPlanDTO value)
        {
            var plan = Mapper.Map<SubscriptionPlan>(value);

            try
            {
                UnitOfWork.SubscriptionPlansRepository.InsertOrUpdate(plan);
                UnitOfWork.Save();

                MemoryCacher.Delete(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/subscriptionPlans/{id}
        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Put(Guid id, [FromBody]SubscriptionPlanDTO value)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var plan = UnitOfWork.SubscriptionPlansRepository.Find(id);
            if (plan == null)
                return NotFound();

            try
            {
                plan.Name = value.Name;
                plan.Description = value.Description;
                plan.Price = value.Price;
                plan.Length = value.Length;
                plan.IsLimited = value.IsLimited;
                plan.MonthlyQuota = value.MonthlyQuota;
                plan.MonthlyDiskSpace = value.MonthlyDiskSpace;
                plan.PdfExport = value.PdfExport;
                plan.ZipExport = value.ZipExport;

                UnitOfWork.SubscriptionPlansRepository.InsertOrUpdate(plan);
                UnitOfWork.Save();

                MemoryCacher.DeleteListAndItem(CACHE_KEY, id);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE api/subscritionPlans/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            try
            {
                UnitOfWork.SubscriptionPlansRepository.Delete(id);
                UnitOfWork.Save();

                MemoryCacher.DeleteListAndItem(CACHE_KEY, id);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
