using AutoMapper;
using LightMethods.Survey.Models.DAL;
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
    public class OrgUserTypesController : BaseApiController
    {

        private const string CACHE_KEY = "ORG_USER_TYPES";

        // GET api/orgUserTypes
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrgUserTypeDTO>))]
        public IHttpActionResult Get()
        {
            var cacheEntry = MemoryCacher.GetValue(CACHE_KEY);
            if (cacheEntry == null)
            {
                var types = new List<OrgUserType> {
                    OrgUserTypesRepository.Administrator,
                    OrgUserTypesRepository.TeamUser
                };

                var result = types
                    .Select(u => Mapper.Map<OrgUserTypeDTO>(u))
                    .ToList();

                MemoryCacher.Add(CACHE_KEY, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (List<OrgUserTypeDTO>)cacheEntry;
                return new CachedResult<List<OrgUserTypeDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

    }
}