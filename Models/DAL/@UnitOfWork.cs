using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services.Identity;
using System;
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
                if (_UserManager == null)
                    _UserManager = new ApplicationUserManager(new ApplicationUserStore(Context));

                return _UserManager;
            }
        }

        private UsersRepository _UsersRepository;
        public UsersRepository UsersRepository
        {
            get
            {
                if (_UsersRepository == null)
                    _UsersRepository = new UsersRepository(this);

                return _UsersRepository;
            }
        }

        private RolesRepository _RolesRepository;
        public RolesRepository RolesRepository
        {
            get
            {
                if (_RolesRepository == null)
                    _RolesRepository = new RolesRepository(this);

                return _RolesRepository;
            }
        }

        private SuperUsersRepository _SuperUsersRepository;
        public SuperUsersRepository SuperUsersRepository
        {
            get
            {
                if (_SuperUsersRepository == null)
                    _SuperUsersRepository = new SuperUsersRepository(this);

                return _SuperUsersRepository;
            }
        }

        private PlatformUsersRepository _PlatformUsersRepository;
        public PlatformUsersRepository PlatformUsersRepository
        {
            get
            {
                if (_PlatformUsersRepository == null)
                    _PlatformUsersRepository = new PlatformUsersRepository(this);

                return _PlatformUsersRepository;
            }
        }

        private OrgUsersRepository _OrgUsersRepository;
        public OrgUsersRepository OrgUsersRepository
        {
            get
            {
                if (_OrgUsersRepository == null)
                    _OrgUsersRepository = new OrgUsersRepository(this);

                return _OrgUsersRepository;
            }
        }

        private OrgUserTypesRepository _OrgUserTypesRepository;
        public OrgUserTypesRepository OrgUserTypesRepository
        {
            get
            {
                if (_OrgUserTypesRepository == null)
                    _OrgUserTypesRepository = new OrgUserTypesRepository(this);

                return _OrgUserTypesRepository;
            }
        }

        private OrganisationRepository _OrganisationRepository;
        public OrganisationRepository OrganisationRepository
        {
            get
            {
                if (_OrganisationRepository == null)
                    _OrganisationRepository = new OrganisationRepository(this);

                return _OrganisationRepository;
            }
        }

        private OrganisationTeamsRepository _OrganisationTeamsRepository;
        public OrganisationTeamsRepository OrganisationTeamsRepository
        {
            get
            {
                if (_OrganisationTeamsRepository == null)
                    _OrganisationTeamsRepository = new OrganisationTeamsRepository(this);

                return _OrganisationTeamsRepository;
            }
        }

        private OrgTeamUsersRepository _OrgTeamUsersRepository;
        public OrgTeamUsersRepository OrgTeamUsersRepository
        {
            get
            {
                if (_OrgTeamUsersRepository == null)
                    _OrgTeamUsersRepository = new OrgTeamUsersRepository(this);

                return _OrgTeamUsersRepository;
            }
        }

        private OrgInvitationsRepository _OrgInvitationsRepository;
        public OrgInvitationsRepository OrgInvitationsRepository
        {
            get
            {
                if (_OrgInvitationsRepository == null)
                    _OrgInvitationsRepository = new OrgInvitationsRepository(this);

                return _OrgInvitationsRepository;
            }
        }

        private OrgConnectionRequestsRepository _OrgConnectionRequestsRepository;
        public OrgConnectionRequestsRepository OrgConnectionRequestsRepository
        {
            get
            {
                if (_OrgConnectionRequestsRepository == null)
                    _OrgConnectionRequestsRepository = new OrgConnectionRequestsRepository(this);

                return _OrgConnectionRequestsRepository;
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
                if (_AssignmentsRepository == null)
                    _AssignmentsRepository = new AssignmentsRepository(this);

                return _AssignmentsRepository;
            }
        }

        private AdultContactNumbersRepository _AdultContactNumbersRepository;
        public AdultContactNumbersRepository AdultContactNumbersRepository
        {
            get
            {
                if (_AdultContactNumbersRepository == null)
                    _AdultContactNumbersRepository = new AdultContactNumbersRepository(this);

                return _AdultContactNumbersRepository;
            }
        }

        private ExternalOrgContactNumbersRepository _ExternalOrgContactNumbersRepository;
        public ExternalOrgContactNumbersRepository ExternalOrgContactNumbersRepository
        {
            get
            {
                if (_ExternalOrgContactNumbersRepository == null)
                    _ExternalOrgContactNumbersRepository = new ExternalOrgContactNumbersRepository(this);

                return _ExternalOrgContactNumbersRepository;
            }
        }

        private ContactNumberTypesRepository _ContactNumberTypesRepository;
        public ContactNumberTypesRepository ContactNumberTypesRepository
        {
            get
            {
                if (_ContactNumberTypesRepository == null)
                    _ContactNumberTypesRepository = new ContactNumberTypesRepository(this);

                return _ContactNumberTypesRepository;
            }
        }

        private AddressTypesRepository _AddressTypesRepository;
        public AddressTypesRepository AddressTypesRepository
        {
            get
            {
                if (_AddressTypesRepository == null)
                    _AddressTypesRepository = new AddressTypesRepository(this);

                return _AddressTypesRepository;
            }
        }

        private AddressesRepository _AddressesRepository;
        public AddressesRepository AddressesRepository
        {
            get
            {
                if (_AddressesRepository == null)
                    _AddressesRepository = new AddressesRepository(this);

                return _AddressesRepository;
            }
        }

        private AdultAddressesRepository _AdultAddressesRepository;
        public AdultAddressesRepository AdultAddressesRepository
        {
            get
            {
                if (_AdultAddressesRepository == null)
                    _AdultAddressesRepository = new AdultAddressesRepository(this);

                return _AdultAddressesRepository;
            }
        }

        private ProjectsRepository _ProjectsRepository;
        public ProjectsRepository ProjectsRepository
        {
            get
            {
                if (_ProjectsRepository == null)
                    _ProjectsRepository = new ProjectsRepository(this);

                return _ProjectsRepository;
            }
        }

        private AdultsRepository _AdultsRepository;
        public AdultsRepository AdultsRepository
        {
            get
            {
                if (_AdultsRepository == null)
                    _AdultsRepository = new AdultsRepository(this);

                return _AdultsRepository;
            }
        }

        private AdultTitlesRepository _AdultTitlesRepository;
        public AdultTitlesRepository AdultTitlesRepository
        {
            get
            {
                if (_AdultTitlesRepository == null)
                    _AdultTitlesRepository = new AdultTitlesRepository(this);

                return _AdultTitlesRepository;
            }
        }

        private KeyLocationsRepository _KeyLocationsRepository;
        public KeyLocationsRepository KeyLocationsRepository
        {
            get
            {
                if (_KeyLocationsRepository == null)
                    _KeyLocationsRepository = new KeyLocationsRepository(this);

                return _KeyLocationsRepository;
            }
        }

        private ExternalOrganisationsRepository _ExternalOrganisationsRepository;
        public ExternalOrganisationsRepository ExternalOrganisationsRepository
        {
            get
            {
                if (_ExternalOrganisationsRepository == null)
                    _ExternalOrganisationsRepository = new ExternalOrganisationsRepository(this);

                return _ExternalOrganisationsRepository;
            }
        }

        private OrganisationWorkersRepository _OrganisationWorkersRepository;
        public OrganisationWorkersRepository OrganisationWorkersRepository
        {
            get
            {
                if (_OrganisationWorkersRepository == null)
                    _OrganisationWorkersRepository = new OrganisationWorkersRepository(this);

                return _OrganisationWorkersRepository;
            }
        }

        private CommentariesRepository _CommentariesRepository;
        public CommentariesRepository CommentariesRepository
        {
            get
            {
                if (_CommentariesRepository == null)
                    _CommentariesRepository = new CommentariesRepository(this);

                return _CommentariesRepository;
            }
        }

        private DocumentsRepository _DocumentsRepository;
        public DocumentsRepository DocumentsRepository
        {
            get
            {
                if (_DocumentsRepository == null)
                    _DocumentsRepository = new DocumentsRepository(this);

                return _DocumentsRepository;
            }
        }

        private FilesRepository _FilesRepository;
        public FilesRepository FilesRepository
        {
            get
            {

                if (_FilesRepository == null)
                    _FilesRepository = new FilesRepository(this);

                return _FilesRepository;
            }
        }

        private GuidanceRepository _GuidanceRepository;
        public GuidanceRepository GuidanceRepository
        {
            get
            {
                if (_GuidanceRepository == null)
                    _GuidanceRepository = new GuidanceRepository(this);

                return _GuidanceRepository;
            }
        }

        private SettingsRepository _SettingsRepository;
        public SettingsRepository SettingsRepository
        {
            get
            {
                if (_SettingsRepository == null)
                    _SettingsRepository = new SettingsRepository(this);

                return _SettingsRepository;
            }
        }

        private SeverityLevelRepository _SeverityLevelRepository;
        public SeverityLevelRepository SeverityLevelRepository
        {
            get
            {

                if (_SeverityLevelRepository == null)
                    _SeverityLevelRepository = new SeverityLevelRepository(this);

                return _SeverityLevelRepository;
            }
        }

        private DataListsRepository _DataListsRepository;
        public DataListsRepository DataListsRepository
        {
            get
            {
                if (_DataListsRepository == null)
                    _DataListsRepository = new DataListsRepository(this);

                return _DataListsRepository;
            }
        }

        private DataListItemsRepository _DataListItemsRepository;
        public DataListItemsRepository DataListItemsRepository
        {
            get
            {
                if (_DataListItemsRepository == null)
                    _DataListItemsRepository = new DataListItemsRepository(this);

                return _DataListItemsRepository;
            }
        }

        private DataListRelationshipsRepository _DataListRelationshipsRepository;
        public DataListRelationshipsRepository DataListRelationshipsRepository
        {
            get
            {
                if (_DataListRelationshipsRepository == null)
                    _DataListRelationshipsRepository = new DataListRelationshipsRepository(this);

                return _DataListRelationshipsRepository;
            }
        }

        private LanguagesRepository _LanguagesRepository;
        public LanguagesRepository LanguagesRepository
        {
            get
            {
                if (_LanguagesRepository == null)
                    _LanguagesRepository = new LanguagesRepository(this);

                return _LanguagesRepository;
            }
        }

        private CalendarsRepository _CalendarsRepository;
        public CalendarsRepository CalendarsRepository
        {
            get
            {
                if (_CalendarsRepository == null)
                    _CalendarsRepository = new CalendarsRepository(this);

                return _CalendarsRepository;
            }
        }

        private PaymentsRepository _PaymentsRepository;
        public PaymentsRepository PaymentsRepository
        {
            get
            {
                if (_PaymentsRepository == null)
                    _PaymentsRepository = new PaymentsRepository(this);

                return _PaymentsRepository;
            }
        }

        private VouchersRepository _VouchersRepository;
        public VouchersRepository VouchersRepository
        {
            get
            {
                if (_VouchersRepository == null)
                    _VouchersRepository = new VouchersRepository(this);

                return _VouchersRepository;
            }
        }

        private SubscriptionPlansRepository _SubscriptionPlansRepository;
        public SubscriptionPlansRepository SubscriptionPlansRepository
        {
            get
            {
                if (_SubscriptionPlansRepository == null)
                    _SubscriptionPlansRepository = new SubscriptionPlansRepository(this);

                return _SubscriptionPlansRepository;
            }
        }

        private SubscriptionsRepository _SubscriptionsRepository;
        public SubscriptionsRepository SubscriptionsRepository
        {
            get
            {
                if (_SubscriptionsRepository == null)
                    _SubscriptionsRepository = new SubscriptionsRepository(this);

                return _SubscriptionsRepository;
            }
        }

        private FeedbacksRepository _FeedbacksRepository;
        public FeedbacksRepository FeedbacksRepository
        {
            get
            {
                if (_FeedbacksRepository == null)
                    _FeedbacksRepository = new FeedbacksRepository(this);

                return _FeedbacksRepository;
            }
        }

        private EmailsRepository _EmailsRepository;
        public EmailsRepository EmailsRepository
        {
            get
            {
                if (_EmailsRepository == null)
                    _EmailsRepository = new EmailsRepository(this);

                return _EmailsRepository;
            }
        }

        private EmailRecipientsRepository _EmailRecipientsRepository;
        public EmailRecipientsRepository EmailRecipientsRepository
        {
            get
            {
                if (_EmailRecipientsRepository == null)
                    _EmailRecipientsRepository = new EmailRecipientsRepository(this);

                return _EmailRecipientsRepository;
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
            if (!disposed)
            {
                if (disposing)
                    Context.SaveChanges();
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
