using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{

    public class OrgUserTypesController : BaseApiController
    {

        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrgUserTypeDTO>))]
        public IHttpActionResult Get()
        {
            var types = new List<OrgUserType> { OrgUserTypesRepository.Administrator, OrgUserTypesRepository.TeamUser }
                .Select(u => Mapper.Map<OrgUserTypeDTO>(u)).ToList();

            return Ok(types);
        }

    }
}