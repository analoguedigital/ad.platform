using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using System;
using System.Web.Http;

namespace WebApi.Controllers
{
    [Authorize]
    public class BaseApiController : ApiController
    {
        protected SurveyContext CurrentDbContext { set; get; }

        public BaseApiController()
        {
            CurrentDbContext = ServiceContext.CurrentDbContext;
        }

        protected UnitOfWork UnitOfWork
        {
            get { return ServiceContext.UnitOfWork; }
        }

        private User _CurrentUser;
        protected User CurrentUser
        {
            get
            {
                if (_CurrentUser == null)
                    _CurrentUser = ServiceContext.CurrentUser;

                return _CurrentUser;
            }
        }

        protected OrgUser CurrentOrgUser
        {
            get { return CurrentUser as OrgUser; }
        }

        public Guid? CurrentOrganisationId
        {
            get
            {
                return (CurrentUser as OrgUser)?.OrganisationId;
            }
        }

        private Organisation _CurrentOrganisation;
        protected Organisation CurrentOrganisation
        {
            get
            {
                if (_CurrentOrganisation == null)
                {
                    if (CurrentUser == null || !(CurrentUser is OrgUser)) return null;
                    _CurrentOrganisation = (CurrentUser as OrgUser).Organisation;
                }

                return _CurrentOrganisation;
            }
        }
    }
}