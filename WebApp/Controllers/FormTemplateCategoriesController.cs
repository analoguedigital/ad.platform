using AutoMapper;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class FormTemplateCategoriesController : BaseApiController
    {

        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<FormTemplateCategoryDTO>))]
        public IHttpActionResult Get()
        {
            if (CurrentOrgUser == null)
                return Unauthorized();

            return Ok(UnitOfWork.FormTemplateCategoriesRepository.AllAsNoTracking
                .Where(c => c.OrganisationId == CurrentOrgUser.OrganisationId)
                .ToList()
                .Select(c => Mapper.Map<FormTemplateCategoryDTO>(c)));
                
        }

        [DeflateCompression]
        [ResponseType(typeof(FormTemplateCategoryDTO))]
        public IHttpActionResult Get(Guid id)
        {
            if (CurrentOrgUser == null)
                return Unauthorized();

            return Ok(UnitOfWork.FormTemplateCategoriesRepository.AllAsNoTracking
                .Where(c => c.Id == id && c.OrganisationId == CurrentOrgUser.OrganisationId)
                .Select(c => Mapper.Map<FormTemplateCategoryDTO>(c))
                .SingleOrDefault());
        }

        [ResponseType(typeof(FormTemplateCategoryDTO))]
        public IHttpActionResult Post(FormTemplateCategoryDTO category)
        {
            var item = new FormTemplateCategory();

            Mapper.Map(category, item);
            item.OrganisationId = CurrentOrgUser.OrganisationId.Value;

            UnitOfWork.FormTemplateCategoriesRepository.InsertOrUpdate(item);
            UnitOfWork.Save();
            return Ok(Mapper.Map<FormTemplateCategoryDTO>(item));
        }

        public IHttpActionResult Put(Guid id, FormTemplateCategoryDTO category)
        {
            var item = UnitOfWork.FormTemplateCategoriesRepository.All
                .Where(c => c.Id == id && c.OrganisationId == CurrentOrgUser.OrganisationId).SingleOrDefault();

            if (item == null)
                return BadRequest("Invalid id");

            Mapper.Map(category, item);
            item.OrganisationId = CurrentOrgUser.OrganisationId.Value;

            UnitOfWork.FormTemplateCategoriesRepository.InsertOrUpdate(item);
            UnitOfWork.Save();
            return Ok();
        }

        public IHttpActionResult Delete(Guid id)
        {
            var item = UnitOfWork.FormTemplateCategoriesRepository.All
                .Where(c => c.Id == id && c.OrganisationId == CurrentOrgUser.OrganisationId).SingleOrDefault();

            if (item == null)
                return BadRequest("Invalid id");

            UnitOfWork.FormTemplateCategoriesRepository.Delete(item);
            UnitOfWork.Save();
            return Ok();
        }
    }
}