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
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator")]
    public class OrganisationsController : BaseApiController
    {
        private OrganisationRepository Organisations { get { return UnitOfWork.OrganisationRepository; } }

        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrganisationDTO>))]
        public IHttpActionResult Get()
        {
            var orgs = Organisations.AllIncluding(u => u.RootUser)
                .OrderBy(u => u.Name)
                .ToList()
                .Select(u => Mapper.Map<OrganisationDTO>(u)).ToList();

            return Ok(orgs);
        }

        [DeflateCompression]
        [ResponseType(typeof(OrganisationDTO))]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<OrganisationDTO>(new Organisation()));

            var org = Mapper.Map<OrganisationDTO>(Organisations.Find(id));

            if (org == null)
                return NotFound();

            return Ok(org);
        }

        public IHttpActionResult Post([FromBody]OrganisationDTO value)
        {
            var createOrganisation = new CreateOrganisation();
            createOrganisation.Name = value.Name;
            createOrganisation.RootUserEmail = value.RootUser.Email;
            createOrganisation.RootPassword = value.RootUser.Password;
            createOrganisation.RootConfirmPassword = value.RootUser.ConfirmPassword;
            createOrganisation.AddressLine1 = value.AddressLine1;
            createOrganisation.AddressLine2 = value.AddressLine2;
            createOrganisation.County = value.County;
            createOrganisation.Town = value.Town;
            createOrganisation.Postcode = value.Postcode;
            createOrganisation.TelNumber = value.TelNumber;
            createOrganisation.DefaultCalendarId = CalendarsRepository.Gregorian.Id;
            createOrganisation.DefaultLanguageId = LanguagesRepository.English.Id;

            Organisations.CreateOrganisation(createOrganisation);
            UnitOfWork.Save();

            return Ok();
        }

        public void Put(Guid id, [FromBody]OrganisationDTO value)
        {

            var org = Organisations.Find(id);
            if (org == null)
                return;

            Mapper.Map(value, org);

            Organisations.InsertOrUpdate(org);
            UnitOfWork.Save();
        }

        public void Delete(Guid id)
        {
            Organisations.Delete(id);
            UnitOfWork.Save();
        }
    }
}