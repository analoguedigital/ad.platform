using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using System;
using System.Web.Http;

namespace WebApi.Controllers
{
    [Authorize]
    public class BaseApiController: ApiController
    {
        public BaseApiController()
        {
            this.CurrentDbContext = ServiceContext.CurrentDbContext;
        }

        protected SurveyContext CurrentDbContext { set; get; }

        protected UnitOfWork UnitOfWork
        {
            get
            {
                return ServiceContext.UnitOfWork;
            }
        }

        private User _CurrentUser;
        protected User CurrentUser
        {
            get
            {
                if (_CurrentUser == null)
                {
                    _CurrentUser = ServiceContext.CurrentUser;
                }
                return _CurrentUser;
            }
        }

        public Guid? CurrentOrganisationId { get { return (CurrentUser as OrgUser)?.OrganisationId; } }

        private Organisation _CurrentOrganisation;
        protected Organisation CurrentOrganisation
        {
            get
            {

                if (_CurrentOrganisation == null)
                {
                    if (CurrentUser == null || !(CurrentUser is OrgUser))
                        return null;

                    _CurrentOrganisation = (CurrentUser as OrgUser).Organisation;
                }
                return _CurrentOrganisation;
            }
        }

        protected OrgUser CurrentOrgUser
        {
            get
            {
                  return CurrentUser as OrgUser;
            }
        }
    }
}