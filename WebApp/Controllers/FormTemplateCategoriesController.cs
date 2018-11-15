using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator,Organisation administrator,Organisation user")]
    public class FormTemplateCategoriesController : BaseApiController
    {

        private const string CACHE_KEY = "FORM_CATEGORIES";

        // GET api/formTemplateCategories
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<FormTemplateCategoryDTO>))]
        public IHttpActionResult Get()
        {
            if (CurrentUser is OrgUser)
            {
                var cacheKey = $"{CACHE_KEY}_{CurrentOrgUser.OrganisationId}";
                var cacheEntry = MemoryCacher.GetValue(cacheKey);

                if (cacheEntry == null)
                {
                    var categories = UnitOfWork.FormTemplateCategoriesRepository
                        .AllAsNoTracking
                        .Where(c => c.OrganisationId == CurrentOrgUser.OrganisationId)
                        .ToList()
                        .Select(c => Mapper.Map<FormTemplateCategoryDTO>(c))
                        .ToList();

                    MemoryCacher.Add(cacheKey, categories, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(categories);
                }
                else
                {
                    var result = (List<FormTemplateCategoryDTO>)cacheEntry;
                    return new CachedResult<List<FormTemplateCategoryDTO>>(result, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is SuperUser
            var _cacheEntry = MemoryCacher.GetValue(CACHE_KEY);
            if (_cacheEntry == null)
            {
                var _categories = UnitOfWork.FormTemplateCategoriesRepository
                    .AllAsNoTracking
                    .ToList()
                    .Select(c => Mapper.Map<FormTemplateCategoryDTO>(c))
                    .ToList();

                MemoryCacher.Add(CACHE_KEY, _categories, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(_categories);
            }
            else
            {
                var retVal = (List<FormTemplateCategoryDTO>)_cacheEntry;
                return new CachedResult<List<FormTemplateCategoryDTO>>(retVal, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/formTemplateCategories/{id}
        [DeflateCompression]
        [ResponseType(typeof(FormTemplateCategoryDTO))]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            if (CurrentUser is OrgUser)
            {
                var cacheKey = $"{CACHE_KEY}_{id}";
                var cacheEntry = MemoryCacher.GetValue(cacheKey);

                if (cacheEntry == null)
                {
                    var category = UnitOfWork.FormTemplateCategoriesRepository
                        .AllAsNoTracking
                        .Where(c => c.Id == id && c.OrganisationId == CurrentOrgUser.OrganisationId)
                        .Select(c => Mapper.Map<FormTemplateCategoryDTO>(c))
                        .SingleOrDefault();

                    MemoryCacher.Add(cacheKey, category, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(category);
                }
                else
                {
                    var result = (FormTemplateCategoryDTO)cacheEntry;
                    return new CachedResult<FormTemplateCategoryDTO>(result, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is SuperUser
            var _cacheKey = $"{CACHE_KEY}_{id}";
            var _cacheEntry = MemoryCacher.GetValue(_cacheKey);

            if (_cacheEntry == null)
            {
                var _category = UnitOfWork.FormTemplateCategoriesRepository
                    .AllAsNoTracking
                    .Where(c => c.Id == id)
                    .Select(c => Mapper.Map<FormTemplateCategoryDTO>(c))
                    .SingleOrDefault();

                MemoryCacher.Add(_cacheKey, _category, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(_category);
            }
            else
            {
                var retVal = (FormTemplateCategoryDTO)_cacheEntry;
                return new CachedResult<FormTemplateCategoryDTO>(retVal, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/formTemplateCategories
        [ResponseType(typeof(FormTemplateCategoryDTO))]
        public IHttpActionResult Post(FormTemplateCategoryDTO category)
        {
            var item = new FormTemplateCategory();

            Mapper.Map(category, item);
            item.OrganisationId = CurrentOrgUser.OrganisationId.Value;

            try
            {
                UnitOfWork.FormTemplateCategoriesRepository.InsertOrUpdate(item);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/formTemplateCategories/{id}
        public IHttpActionResult Put(Guid id, FormTemplateCategoryDTO category)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var item = UnitOfWork.FormTemplateCategoriesRepository
                .AllAsNoTracking
                .Where(c => c.Id == id && c.OrganisationId == CurrentOrgUser.OrganisationId)
                .SingleOrDefault();

            if (item == null)
                return NotFound();

            Mapper.Map(category, item);
            item.OrganisationId = CurrentOrgUser.OrganisationId.Value;

            try
            {
                UnitOfWork.FormTemplateCategoriesRepository.InsertOrUpdate(item);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DEL api/formTemplateCategories/{id}
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var item = UnitOfWork.FormTemplateCategoriesRepository
                .AllAsNoTracking
                .Where(c => c.Id == id && c.OrganisationId == CurrentOrgUser.OrganisationId)
                .SingleOrDefault();

            if (item == null)
                return NotFound();

            try
            {
                UnitOfWork.FormTemplateCategoriesRepository.Delete(item);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}