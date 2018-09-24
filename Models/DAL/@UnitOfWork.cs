using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services.Identity;
using System.Data.Entity;

namespace LightMethods.Survey.Models.DAL
{
    public partial class UnitOfWork : IDisposable
    {
        public SurveyContext Context { internal set; get; }
        public UnitOfWork(SurveyContext context)
        {
            Context = context;
        }

        public Repository<T> GetGenericRepository<T>() where T : Entity, new()
        {
            return new Repository<T>(this);
        }

        public DbContextTransaction StartTransaction()
        {
            return Context.Database.BeginTransaction();
        }

        private ApplicationUserManager _UserManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                if (this._UserManager == null)
                {
                    this._UserManager = new ApplicationUserManager(new ApplicationUserStore(Context));
                }
                return _UserManager;
            }
        }

        private UsersRepository _UsersRepository;
        public UsersRepository UsersRepository
        {
            get
            {
                if (this._UsersRepository == null)
                {
                    this._UsersRepository = new UsersRepository(this);
                }
                return _UsersRepository;
            }
        }

        private RolesRepository _RolesRepository;
        public RolesRepository RolesRepository
        {
            get
            {
                if (this._RolesRepository == null)
                {
                    this._RolesRepository = new RolesRepository(this);
                }
                return _RolesRepository;
            }
        }

        private SuperUsersRepository _SuperUsersRepository;
        public SuperUsersRepository SuperUsersRepository
        {
            get
            {
                if (this._SuperUsersRepository == null)
                {
                    this._SuperUsersRepository = new SuperUsersRepository(this);
                }
                return _SuperUsersRepository;
            }
        }

        private OrgUsersRepository _OrgUsersRepository;
        public OrgUsersRepository OrgUsersRepository
        {
            get
            {
                if (this._OrgUsersRepository == null)
                {
                    this._OrgUsersRepository = new OrgUsersRepository(this);
                }
                return _OrgUsersRepository;
            }
        }

        private OrgUserTypesRepository _OrgUserTypesRepository;
        public OrgUserTypesRepository OrgUserTypesRepository
        {
            get
            {
                if (this._OrgUserTypesRepository == null)
                {
                    this._OrgUserTypesRepository = new OrgUserTypesRepository(this);
                }
                return _OrgUserTypesRepository;
            }
        }

        private OrganisationRepository _OrganisationRepository;
        public OrganisationRepository OrganisationRepository
        {
            get
            {
                if (this._OrganisationRepository == null)
                {
                    this._OrganisationRepository = new OrganisationRepository(this);
                }
                return _OrganisationRepository;
            }
        }

        private OrganisationTeamsRepository _OrganisationTeamsRepository;
        public OrganisationTeamsRepository OrganisationTeamsRepository
        {
            get
            {
                if (this._OrganisationTeamsRepository == null)
                    this._OrganisationTeamsRepository = new OrganisationTeamsRepository(this);

                return _OrganisationTeamsRepository;
            }
        }

        private OrgTeamUsersRepository _OrgTeamUsersRepository;
        public OrgTeamUsersRepository OrgTeamUsersRepository
        {
            get
            {
                if (this._OrgTeamUsersRepository == null)
                    this._OrgTeamUsersRepository = new OrgTeamUsersRepository(this);

                return _OrgTeamUsersRepository;
            }
        }

        private OrgInvitationsRepository _OrgInvitationsRepository;
        public OrgInvitationsRepository OrgInvitationsRepository
        {
            get
            {
                if (this._OrgInvitationsRepository == null)
                    this._OrgInvitationsRepository = new OrgInvitationsRepository(this);

                return this._OrgInvitationsRepository;
            }
        }

        private OrgConnectionRequestsRepository _OrgConnectionRequestsRepository;
        public OrgConnectionRequestsRepository OrgConnectionRequestsRepository
        {
            get
            {
                if (this._OrgConnectionRequestsRepository == null)
                    this._OrgConnectionRequestsRepository = new OrgConnectionRequestsRepository(this);

                return this._OrgConnectionRequestsRepository;
            }
        }

        private OrgRequestsRepository _OrgRequestsRepository;
        public OrgRequestsRepository OrgRequestsRepository
        {
            get
            {
                if (_OrgRequestsRepository == null)
                    _OrgRequestsRepository = new OrgRequestsRepository(this);

                return _OrgRequestsRepository;
            }
        }

        private AssignmentsRepository _AssignmentsRepository;
        public AssignmentsRepository AssignmentsRepository
        {
            get
            {
                if (this._AssignmentsRepository == null)
                {
                    this._AssignmentsRepository = new AssignmentsRepository(this);
                }
                return _AssignmentsRepository;
            }
        }

        private AdultContactNumbersRepository _AdultContactNumbersRepository;
        public AdultContactNumbersRepository AdultContactNumbersRepository
        {
            get
            {
                if (this._AdultContactNumbersRepository == null)
                {
                    this._AdultContactNumbersRepository = new AdultContactNumbersRepository(this);
                }
                return _AdultContactNumbersRepository;
            }
        }

        private ExternalOrgContactNumbersRepository _ExternalOrgContactNumbersRepository;
        public ExternalOrgContactNumbersRepository ExternalOrgContactNumbersRepository
        {
            get
            {
                if (this._ExternalOrgContactNumbersRepository == null)
                {
                    this._ExternalOrgContactNumbersRepository = new ExternalOrgContactNumbersRepository(this);
                }
                return _ExternalOrgContactNumbersRepository;
            }
        }

        private ContactNumberTypesRepository _ContactNumberTypesRepository;
        public ContactNumberTypesRepository ContactNumberTypesRepository
        {
            get
            {
                if (this._ContactNumberTypesRepository == null)
                {
                    this._ContactNumberTypesRepository = new ContactNumberTypesRepository(this);
                }
                return _ContactNumberTypesRepository;
            }
        }

        private AddressTypesRepository _AddressTypesRepository;
        public AddressTypesRepository AddressTypesRepository
        {
            get
            {
                if (this._AddressTypesRepository == null)
                {
                    this._AddressTypesRepository = new AddressTypesRepository(this);
                }
                return _AddressTypesRepository;
            }
        }

        private AddressesRepository _AddressesRepository;
        public AddressesRepository AddressesRepository
        {
            get
            {
                if (this._AddressesRepository == null)
                {
                    this._AddressesRepository = new AddressesRepository(this);
                }
                return _AddressesRepository;
            }
        }

        private AdultAddressesRepository _AdultAddressesRepository;
        public AdultAddressesRepository AdultAddressesRepository
        {
            get
            {
                if (this._AdultAddressesRepository == null)
                {
                    this._AdultAddressesRepository = new AdultAddressesRepository(this);
                }
                return _AdultAddressesRepository;
            }
        }

        private ProjectsRepository _ProjectsRepository;
        public ProjectsRepository ProjectsRepository
        {
            get
            {
                if (this._ProjectsRepository == null)
                {
                    this._ProjectsRepository = new ProjectsRepository(this);
                }
                return _ProjectsRepository;
            }
        }

        private AdultsRepository _AdultsRepository;
        public AdultsRepository AdultsRepository
        {
            get
            {
                if (this._AdultsRepository == null)
                {
                    this._AdultsRepository = new AdultsRepository(this);
                }
                return _AdultsRepository;
            }
        }

        private AdultTitlesRepository _AdultTitlesRepository;
        public AdultTitlesRepository AdultTitlesRepository
        {
            get
            {
                if (this._AdultTitlesRepository == null)
                {
                    this._AdultTitlesRepository = new AdultTitlesRepository(this);
                }
                return _AdultTitlesRepository;
            }
        }

        private KeyLocationsRepository _KeyLocationsRepository;
        public KeyLocationsRepository KeyLocationsRepository
        {
            get
            {
                if (this._KeyLocationsRepository == null)
                {
                    this._KeyLocationsRepository = new KeyLocationsRepository(this);
                }
                return _KeyLocationsRepository;
            }
        }

        private ExternalOrganisationsRepository _ExternalOrganisationsRepository;
        public ExternalOrganisationsRepository ExternalOrganisationsRepository
        {
            get
            {
                if (this._ExternalOrganisationsRepository == null)
                {
                    this._ExternalOrganisationsRepository = new ExternalOrganisationsRepository(this);
                }
                return _ExternalOrganisationsRepository;
            }
        }

        private OrganisationWorkersRepository _OrganisationWorkersRepository;
        public OrganisationWorkersRepository OrganisationWorkersRepository
        {
            get
            {
                if (this._OrganisationWorkersRepository == null)
                {
                    this._OrganisationWorkersRepository = new OrganisationWorkersRepository(this);
                }
                return _OrganisationWorkersRepository;
            }
        }

        private CommentariesRepository _CommentariesRepository;
        public CommentariesRepository CommentariesRepository
        {
            get
            {
                if (this._CommentariesRepository == null)
                {
                    this._CommentariesRepository = new CommentariesRepository(this);
                }
                return _CommentariesRepository;
            }
        }

        private DocumentsRepository _DocumentsRepository;
        public DocumentsRepository DocumentsRepository
        {
            get
            {
                if (this._DocumentsRepository == null)
                {
                    this._DocumentsRepository = new DocumentsRepository(this);
                }
                return _DocumentsRepository;
            }
        }

        private FilesRepository _FilesRepository;
        public FilesRepository FilesRepository
        {
            get
            {

                if (this._FilesRepository == null)
                {
                    this._FilesRepository = new FilesRepository(this);
                }
                return _FilesRepository;
            }
        }

        private GuidanceRepository _GuidanceRepository;
        public GuidanceRepository GuidanceRepository
        {
            get
            {
                if (this._GuidanceRepository == null)
                {
                    this._GuidanceRepository = new GuidanceRepository(this);
                }
                return _GuidanceRepository;
            }
        }

        private SettingsRepository _SettingsRepository;
        public SettingsRepository SettingsRepository
        {
            get
            {
                if (this._SettingsRepository == null)
                {
                    this._SettingsRepository = new SettingsRepository(this);
                }
                return _SettingsRepository;
            }
        }

        private SeverityLevelRepository _SeverityLevelRepository;
        public SeverityLevelRepository SeverityLevelRepository
        {
            get
            {

                if (this._SeverityLevelRepository == null)
                {
                    this._SeverityLevelRepository = new SeverityLevelRepository(this);
                }
                return _SeverityLevelRepository;
            }
        }

        private DataListsRepository _DataListsRepository;
        public DataListsRepository DataListsRepository
        {
            get
            {
                if (this._DataListsRepository == null)
                {
                    this._DataListsRepository = new DataListsRepository(this);
                }
                return _DataListsRepository;
            }
        }

        private DataListItemsRepository _DataListItemsRepository;
        public DataListItemsRepository DataListItemsRepository
        {
            get
            {
                if (this._DataListItemsRepository == null)
                {
                    this._DataListItemsRepository = new DataListItemsRepository(this);
                }
                return _DataListItemsRepository;
            }
        }

        private DataListRelationshipsRepository _DataListRelationshipsRepository;
        public DataListRelationshipsRepository DataListRelationshipsRepository
        {
            get
            {
                if (this._DataListRelationshipsRepository == null)
                {
                    this._DataListRelationshipsRepository = new DataListRelationshipsRepository(this);
                }
                return _DataListRelationshipsRepository;
            }
        }

        private LanguagesRepository _LanguagesRepository;
        public LanguagesRepository LanguagesRepository
        {
            get
            {
                if (this._LanguagesRepository == null)
                {
                    this._LanguagesRepository = new LanguagesRepository(this);
                }
                return _LanguagesRepository;
            }
        }

        private CalendarsRepository _CalendarsRepository;
        public CalendarsRepository CalendarsRepository
        {
            get
            {
                if (this._CalendarsRepository == null)
                {
                    this._CalendarsRepository = new CalendarsRepository(this);
                }
                return _CalendarsRepository;
            }
        }

        private PaymentsRepository _PaymentsRepository;
        public PaymentsRepository PaymentsRepository
        {
            get
            {
                if (this._PaymentsRepository == null)
                    this._PaymentsRepository = new PaymentsRepository(this);

                return this._PaymentsRepository;
            }
        }

        private VouchersRepository _VouchersRepository;
        public VouchersRepository VouchersRepository
        {
            get
            {
                if (this._VouchersRepository == null)
                    this._VouchersRepository = new VouchersRepository(this);

                return this._VouchersRepository;
            }
        }

        private SubscriptionPlansRepository _SubscriptionPlansRepository;
        public SubscriptionPlansRepository SubscriptionPlansRepository
        {
            get
            {
                if (this._SubscriptionPlansRepository == null)
                    this._SubscriptionPlansRepository = new SubscriptionPlansRepository(this);

                return this._SubscriptionPlansRepository;
            }
        }

        private SubscriptionsRepository _SubscriptionsRepository;
        public SubscriptionsRepository SubscriptionsRepository
        {
            get
            {
                if (this._SubscriptionsRepository == null)
                    this._SubscriptionsRepository = new SubscriptionsRepository(this);

                return this._SubscriptionsRepository;
            }
        }

        private FeedbacksRepository _FeedbacksRepository;
        public FeedbacksRepository FeedbacksRepository
        {
            get
            {
                if (this._FeedbacksRepository == null)
                    this._FeedbacksRepository = new FeedbacksRepository(this);

                return this._FeedbacksRepository;
            }
        }

        private EmailsRepository _EmailsRepository;
        public EmailsRepository EmailsRepository
        {
            get
            {
                if (this._EmailsRepository == null)
                    this._EmailsRepository = new EmailsRepository(this);

                return this._EmailsRepository;
            }
        }

        private EmailRecipientsRepository _EmailRecipientsRepository;
        public EmailRecipientsRepository EmailRecipientsRepository
        {
            get
            {
                if (this._EmailRecipientsRepository == null)
                    this._EmailRecipientsRepository = new EmailRecipientsRepository(this);

                return this._EmailRecipientsRepository;
            }
        }

        public void Save(bool disableValidation = false)
        {
            if (disableValidation)
                Context.Configuration.ValidateOnSaveEnabled = false;

            Context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.SaveChanges();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
