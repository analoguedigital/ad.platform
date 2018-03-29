using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;

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