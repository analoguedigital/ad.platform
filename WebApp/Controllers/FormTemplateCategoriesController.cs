﻿using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;

namespace WebApi.Controllers
{
    public class FormTemplateCategoriesController : BaseApiController
    {
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<FormTemplateCategoryDTO>))]
        public IHttpActionResult Get()
        {
            var result = new List<FormTemplateCategoryDTO>();

            if (CurrentOrgUser != null)
            {
                result = UnitOfWork.FormTemplateCategoriesRepository.AllAsNoTracking
                    .Where(c => c.OrganisationId == CurrentOrgUser.OrganisationId)
                    .ToList()
                    .Select(c => Mapper.Map<FormTemplateCategoryDTO>(c))
                    .ToList();
            }
            else
            {
                result = UnitOfWork.FormTemplateCategoriesRepository.AllAsNoTracking
                    .ToList()
                    .Select(c => Mapper.Map<FormTemplateCategoryDTO>(c))
                    .ToList();
            }

            return Ok(result);

        }

        [DeflateCompression]
        [ResponseType(typeof(FormTemplateCategoryDTO))]
        public IHttpActionResult Get(Guid id)
        {
            var result = new FormTemplateCategoryDTO();

            if (CurrentOrgUser != null)
            {
                result = UnitOfWork.FormTemplateCategoriesRepository.AllAsNoTracking
                    .Where(c => c.Id == id && c.OrganisationId == CurrentOrgUser.OrganisationId)
                    .Select(c => Mapper.Map<FormTemplateCategoryDTO>(c))
                    .SingleOrDefault();
            }
            else
            {
                result = UnitOfWork.FormTemplateCategoriesRepository.AllAsNoTracking
                    .Where(c => c.Id == id)
                    .Select(c => Mapper.Map<FormTemplateCategoryDTO>(c))
                    .SingleOrDefault();
            }

            return Ok(result);
        }

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

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Put(Guid id, FormTemplateCategoryDTO category)
        {
            var item = UnitOfWork.FormTemplateCategoriesRepository.All
                .Where(c => c.Id == id && c.OrganisationId == CurrentOrgUser.OrganisationId).SingleOrDefault();

            if (item == null)
                return BadRequest("Invalid id");

            Mapper.Map(category, item);
            item.OrganisationId = CurrentOrgUser.OrganisationId.Value;

            try
            {
                UnitOfWork.FormTemplateCategoriesRepository.InsertOrUpdate(item);
                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Delete(Guid id)
        {
            var item = UnitOfWork.FormTemplateCategoriesRepository.All
                .Where(c => c.Id == id && c.OrganisationId == CurrentOrgUser.OrganisationId)
                .SingleOrDefault();

            if (item == null)
                return BadRequest("Invalid id");

            try
            {
                UnitOfWork.FormTemplateCategoriesRepository.Delete(item);
                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}