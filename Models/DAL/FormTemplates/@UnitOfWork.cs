using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMethods.Survey.Models.DAL
{
    public partial class UnitOfWork
    {
        private FormTemplateCategoriesRepository _FormTemplateCategoriesRepository;
        public FormTemplateCategoriesRepository FormTemplateCategoriesRepository
        {
            get
            {

                if (this._FormTemplateCategoriesRepository == null)
                {
                    this._FormTemplateCategoriesRepository = new FormTemplateCategoriesRepository(this);
                }
                return _FormTemplateCategoriesRepository;
            }
        }

        private FormTemplatesRepository _FormTemplatesRepository;
        public FormTemplatesRepository FormTemplatesRepository
        {
            get
            {

                if (this._FormTemplatesRepository == null)
                {
                    this._FormTemplatesRepository = new FormTemplatesRepository(this);
                }
                return _FormTemplatesRepository;
            }
        }

        private MetricGroupsRepository _MetricGroupsRepository;
        public MetricGroupsRepository MetricGroupsRepository
        {
            get
            {

                if (this._MetricGroupsRepository == null)
                {
                    this._MetricGroupsRepository = new MetricGroupsRepository(this);
                }
                return _MetricGroupsRepository;
            }
        }

        private MetricsRepository _MetricsRepository;
        public MetricsRepository MetricsRepository
        {
            get
            {

                if (this._MetricsRepository == null)
                {
                    this._MetricsRepository = new MetricsRepository(this);
                }
                return _MetricsRepository;
            }
        }

        private AttachmentTypesRepository _AttachmentTypesRepository;
        public AttachmentTypesRepository AttachmentTypesRepository
        {
            get
            {

                if (this._AttachmentTypesRepository == null)
                {
                    this._AttachmentTypesRepository = new AttachmentTypesRepository(this);
                }
                return _AttachmentTypesRepository;
            }
        }

        private AttachmentMetricsRepository _AttachmentMetricsRepository;
        public AttachmentMetricsRepository AttachmentMetricsRepository
        {
            get
            {

                if (this._AttachmentMetricsRepository == null)
                {
                    this._AttachmentMetricsRepository = new AttachmentMetricsRepository(this);
                }
                return _AttachmentMetricsRepository;
            }
        }

        private AttachmentsRepository _AttachmentsRepository;
        public AttachmentsRepository AttachmentsRepository
        {
            get
            {

                if (this._AttachmentsRepository == null)
                {
                    this._AttachmentsRepository = new AttachmentsRepository(this);
                }
                return _AttachmentsRepository;
            }
        }
        private FreeTextMetricsRepository _FreeTextMetricsRepository;
        public FreeTextMetricsRepository FreeTextMetricsRepository
        {
            get
            {

                if (this._FreeTextMetricsRepository == null)
                {
                    this._FreeTextMetricsRepository = new FreeTextMetricsRepository(this);
                }
                return _FreeTextMetricsRepository;
            }
        }

        private RateMetricsRepository _RateMetricsRepository;
        public RateMetricsRepository RateMetricsRepository
        {
            get
            {

                if (this._RateMetricsRepository == null)
                {
                    this._RateMetricsRepository = new RateMetricsRepository(this);
                }
                return _RateMetricsRepository;
            }
        }

        private DateMetricsRepository _DateMetricsRepository;
        public DateMetricsRepository DateMetricsRepository
        {
            get
            {

                if (this._DateMetricsRepository == null)
                {
                    this._DateMetricsRepository = new DateMetricsRepository(this);
                }
                return _DateMetricsRepository;
            }
        }

        private TimeMetricsRepository _TimeMetricsRepository;
        public TimeMetricsRepository TimeMetricsRepository
        {
            get
            {

                if (this._TimeMetricsRepository == null)
                {
                    this._TimeMetricsRepository = new TimeMetricsRepository(this);
                }
                return _TimeMetricsRepository;
            }
        }

        private DichotomousMetricsRepository _DichotomousMetricsRepository;
        public DichotomousMetricsRepository DichotomousMetricsRepository
        {
            get
            {

                if (this._DichotomousMetricsRepository == null)
                {
                    this._DichotomousMetricsRepository = new DichotomousMetricsRepository(this);
                }
                return _DichotomousMetricsRepository;
            }
        }

        private NumericMetricsRepository _NumericMetricsRepository;
        public NumericMetricsRepository NumericMetricsRepository
        {
            get
            {

                if (this._NumericMetricsRepository == null)
                {
                    this._NumericMetricsRepository = new NumericMetricsRepository(this);
                }
                return _NumericMetricsRepository;
            }
        }

        private MultipleChoiceMetricsRepository _MultipleChoiceMetricsRepository;
        public MultipleChoiceMetricsRepository MultipleChoiceMetricsRepository
        {
            get
            {

                if (this._MultipleChoiceMetricsRepository == null)
                {
                    this._MultipleChoiceMetricsRepository = new MultipleChoiceMetricsRepository(this);
                }
                return _MultipleChoiceMetricsRepository;
            }
        }

        private FilledFormsRepository _FilledFormsRepository;
        public FilledFormsRepository FilledFormsRepository
        {
            get
            {

                if (this._FilledFormsRepository == null)
                {
                    this._FilledFormsRepository = new FilledFormsRepository(this);
                }
                return _FilledFormsRepository;
            }
        }

        private FormValuesRepository _FormValuesRepository;
        public FormValuesRepository FormValuesRepository
        {
            get
            {

                if (this._FormValuesRepository == null)
                {
                    this._FormValuesRepository = new FormValuesRepository(this);
                }
                return _FormValuesRepository;
            }
        }

        private ThreadAssignmentsRepository _ThreadAssignmentsRepository;
        public ThreadAssignmentsRepository ThreadAssignmentsRepository
        {
            get
            {

                if (this._ThreadAssignmentsRepository == null)
                    this._ThreadAssignmentsRepository = new ThreadAssignmentsRepository(this);

                return _ThreadAssignmentsRepository;
            }
        }
    }
}
