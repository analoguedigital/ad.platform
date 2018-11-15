namespace LightMethods.Survey.Models.DAL
{
    public partial class UnitOfWork
    {
        private FormTemplateCategoriesRepository _FormTemplateCategoriesRepository;
        public FormTemplateCategoriesRepository FormTemplateCategoriesRepository
        {
            get
            {
                if (_FormTemplateCategoriesRepository == null)
                    _FormTemplateCategoriesRepository = new FormTemplateCategoriesRepository(this);

                return _FormTemplateCategoriesRepository;
            }
        }

        private FormTemplatesRepository _FormTemplatesRepository;
        public FormTemplatesRepository FormTemplatesRepository
        {
            get
            {
                if (_FormTemplatesRepository == null)
                    _FormTemplatesRepository = new FormTemplatesRepository(this);

                return _FormTemplatesRepository;
            }
        }

        private MetricGroupsRepository _MetricGroupsRepository;
        public MetricGroupsRepository MetricGroupsRepository
        {
            get
            {
                if (_MetricGroupsRepository == null)
                    _MetricGroupsRepository = new MetricGroupsRepository(this);

                return _MetricGroupsRepository;
            }
        }

        private MetricsRepository _MetricsRepository;
        public MetricsRepository MetricsRepository
        {
            get
            {
                if (_MetricsRepository == null)
                    _MetricsRepository = new MetricsRepository(this);

                return _MetricsRepository;
            }
        }

        private AttachmentTypesRepository _AttachmentTypesRepository;
        public AttachmentTypesRepository AttachmentTypesRepository
        {
            get
            {
                if (_AttachmentTypesRepository == null)
                    _AttachmentTypesRepository = new AttachmentTypesRepository(this);

                return _AttachmentTypesRepository;
            }
        }

        private AttachmentMetricsRepository _AttachmentMetricsRepository;
        public AttachmentMetricsRepository AttachmentMetricsRepository
        {
            get
            {
                if (_AttachmentMetricsRepository == null)
                    _AttachmentMetricsRepository = new AttachmentMetricsRepository(this);

                return _AttachmentMetricsRepository;
            }
        }

        private AttachmentsRepository _AttachmentsRepository;
        public AttachmentsRepository AttachmentsRepository
        {
            get
            {
                if (_AttachmentsRepository == null)
                    _AttachmentsRepository = new AttachmentsRepository(this);

                return _AttachmentsRepository;
            }
        }
        private FreeTextMetricsRepository _FreeTextMetricsRepository;
        public FreeTextMetricsRepository FreeTextMetricsRepository
        {
            get
            {
                if (_FreeTextMetricsRepository == null)
                    _FreeTextMetricsRepository = new FreeTextMetricsRepository(this);

                return _FreeTextMetricsRepository;
            }
        }

        private RateMetricsRepository _RateMetricsRepository;
        public RateMetricsRepository RateMetricsRepository
        {
            get
            {
                if (_RateMetricsRepository == null)
                    _RateMetricsRepository = new RateMetricsRepository(this);

                return _RateMetricsRepository;
            }
        }

        private DateMetricsRepository _DateMetricsRepository;
        public DateMetricsRepository DateMetricsRepository
        {
            get
            {
                if (_DateMetricsRepository == null)
                    _DateMetricsRepository = new DateMetricsRepository(this);

                return _DateMetricsRepository;
            }
        }

        private TimeMetricsRepository _TimeMetricsRepository;
        public TimeMetricsRepository TimeMetricsRepository
        {
            get
            {
                if (_TimeMetricsRepository == null)
                    _TimeMetricsRepository = new TimeMetricsRepository(this);

                return _TimeMetricsRepository;
            }
        }

        private DichotomousMetricsRepository _DichotomousMetricsRepository;
        public DichotomousMetricsRepository DichotomousMetricsRepository
        {
            get
            {
                if (_DichotomousMetricsRepository == null)
                    _DichotomousMetricsRepository = new DichotomousMetricsRepository(this);

                return _DichotomousMetricsRepository;
            }
        }

        private NumericMetricsRepository _NumericMetricsRepository;
        public NumericMetricsRepository NumericMetricsRepository
        {
            get
            {
                if (_NumericMetricsRepository == null)
                    _NumericMetricsRepository = new NumericMetricsRepository(this);

                return _NumericMetricsRepository;
            }
        }

        private MultipleChoiceMetricsRepository _MultipleChoiceMetricsRepository;
        public MultipleChoiceMetricsRepository MultipleChoiceMetricsRepository
        {
            get
            {
                if (_MultipleChoiceMetricsRepository == null)
                    _MultipleChoiceMetricsRepository = new MultipleChoiceMetricsRepository(this);

                return _MultipleChoiceMetricsRepository;
            }
        }

        private FilledFormsRepository _FilledFormsRepository;
        public FilledFormsRepository FilledFormsRepository
        {
            get
            {
                if (_FilledFormsRepository == null)
                    _FilledFormsRepository = new FilledFormsRepository(this);

                return _FilledFormsRepository;
            }
        }

        private FormValuesRepository _FormValuesRepository;
        public FormValuesRepository FormValuesRepository
        {
            get
            {
                if (_FormValuesRepository == null)
                    _FormValuesRepository = new FormValuesRepository(this);

                return _FormValuesRepository;
            }
        }

        private ThreadAssignmentsRepository _ThreadAssignmentsRepository;
        public ThreadAssignmentsRepository ThreadAssignmentsRepository
        {
            get
            {
                if (_ThreadAssignmentsRepository == null)
                    _ThreadAssignmentsRepository = new ThreadAssignmentsRepository(this);

                return _ThreadAssignmentsRepository;
            }
        }
    }
}
