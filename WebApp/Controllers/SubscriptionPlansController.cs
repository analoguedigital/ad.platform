using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    [RoutePrefix("api/subscriptionplans")]
    public class SubscriptionPlansController : BaseApiController
    {
        public IHttpActionResult Get()
        {
            var plans = UnitOfWork.SubscriptionPlansRepository.AllAsNoTracking;
            var result = plans.ToList().Select(sp => Mapper.Map<SubscriptionPlanDTO>(sp));

            return Ok(result);
        }

        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var plan = UnitOfWork.SubscriptionPlansRepository.Find(id);
            if (plan == null)
                return NotFound();

            return Ok(Mapper.Map<SubscriptionPlanDTO>(plan));
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody]SubscriptionPlanDTO value)
        {
            var plan = Mapper.Map<SubscriptionPlan>(value);
            try
            {
                UnitOfWork.SubscriptionPlansRepository.InsertOrUpdate(plan);
                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Put(Guid id, [FromBody]SubscriptionPlanDTO value)
        {
            if (id == Guid.Empty)
                return BadRequest();

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
                plan.PdfExport = value.PdfExport;
                plan.ZipExport = value.ZipExport;

                UnitOfWork.SubscriptionPlansRepository.InsertOrUpdate(plan);
                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                UnitOfWork.SubscriptionPlansRepository.Delete(id);
                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
