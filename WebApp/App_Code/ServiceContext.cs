using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;

namespace WebApi
{
    public class ServiceContext
    {
        static HttpContext Context { get { return HttpContext.Current; } }

        public static ApplicationSignInManager SignInManager
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;

                if (HttpContext.Current.Items["current.context.signinmanager"] == null)
                    HttpContext.Current.Items["current.context.signinmanager"] = Context.GetOwinContext().Get<ApplicationSignInManager>();

                return HttpContext.Current.Items["current.context.signinmanager"] as ApplicationSignInManager;
            }
        }

        public static ApplicationUserManager UserManager
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;

                if (HttpContext.Current.Items["current.context.usermanager"] == null)
                    HttpContext.Current.Items["current.context.usermanager"] = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

                return HttpContext.Current.Items["current.context.usermanager"] as ApplicationUserManager;
            }
        }

        public static SurveyContext CurrentDbContext
        {
            get
            {
                if (HttpContext.Current == null)
                    return new SurveyContext();

                if (HttpContext.Current.Items["current.context"] == null)
                    HttpContext.Current.Items["current.context"] = new SurveyContext();
                return (SurveyContext)HttpContext.Current.Items["current.context"];
            }
        }

        public static UnitOfWork UnitOfWork
        {
            get
            {
                if (Context.Items["current.uow"] == null)
                    Context.Items["current.uow"] = new UnitOfWork(CurrentDbContext);

                return (UnitOfWork)Context.Items["current.uow"];
            }
        }

        public static User CurrentUser
        {
            get
            {
                if (Context.Items["current.user"] == null)
                    Context.Items["current.user"] = CreateCurrentUser();
                return (User)Context.Items["current.user"];
            }

        }

        public static Calendar CurrentCalendar
        {
            get
            {
                if (CurrentUser != null && CurrentUser is OrgUser)
                    return ((OrgUser)CurrentUser).Organisation.DefaultCalendar;

                return CalendarsRepository.Gregorian;
            }
        }

        public static Language CurrentLanguage
        {
            get
            {
                if (CurrentUser != null && CurrentUser is OrgUser)
                    return ((OrgUser)CurrentUser).Organisation.DefaultLanguage;

                return LanguagesRepository.English;
            }
        }

        public static User CreateCurrentUser()
        {
            if (!Context.GetOwinContext().Authentication.User.Identity.IsAuthenticated)
                return null;

            return UnitOfWork.UsersRepository.GetUserByEmail(Context.GetOwinContext().Authentication.User.Identity.Name);
        }
    }
}