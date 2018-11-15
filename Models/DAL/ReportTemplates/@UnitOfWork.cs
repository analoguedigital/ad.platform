namespace LightMethods.Survey.Models.DAL
{
    public partial class UnitOfWork
    {
        private ReportsRepository _ReportsRepository;
        public ReportsRepository ReportsRepository
        {
            get
            {

                if (_ReportsRepository == null)
                    _ReportsRepository = new ReportsRepository(this);

                return _ReportsRepository;
            }
        }

        private ReportTemplateCategoriesRepository _ReportTemplateCategoriesRepository;
        public ReportTemplateCategoriesRepository ReportTemplateCategoriesRepository
        {
            get
            {

                if (_ReportTemplateCategoriesRepository == null)
                    _ReportTemplateCategoriesRepository = new ReportTemplateCategoriesRepository(this);

                return _ReportTemplateCategoriesRepository;
            }
        }

        private ReportTemplatesRepository _ReportTemplatesRepository;
        public ReportTemplatesRepository ReportTemplatesRepository
        {
            get
            {

                if (_ReportTemplatesRepository == null)
                    _ReportTemplatesRepository = new ReportTemplatesRepository(this);

                return _ReportTemplatesRepository;
            }
        }

        private ReportChartsRepository _ReportChartsRepository;
        public ReportChartsRepository ReportChartsRepository
        {
            get
            {

                if (_ReportChartsRepository == null)
                    _ReportChartsRepository = new ReportChartsRepository(this);

                return _ReportChartsRepository;
            }
        }

        private ChartSerieTypesRepository _ChartSerieTypesRepository;
        public ChartSerieTypesRepository ChartSerieTypesRepository
        {
            get
            {

                if (_ChartSerieTypesRepository == null)
                    _ChartSerieTypesRepository = new ChartSerieTypesRepository(this);

                return _ChartSerieTypesRepository;
            }
        }

        private ChartSeriesRepository _ChartSeriesRepository;
        public ChartSeriesRepository ChartSeriesRepository
        {
            get
            {

                if (_ChartSeriesRepository == null)
                    _ChartSeriesRepository = new ChartSeriesRepository(this);

                return _ChartSeriesRepository;
            }
        }

        private ReportTablesRepository _ReportTablesRepository;
        public ReportTablesRepository ReportTablesRepository
        {
            get
            {

                if (_ReportTablesRepository == null)
                    _ReportTablesRepository = new ReportTablesRepository(this);

                return _ReportTablesRepository;
            }
        }

        private TableColumnsRepository _TableColumnsRepository;
        public TableColumnsRepository TableColumnsRepository
        {
            get
            {

                if (_TableColumnsRepository == null)
                    _TableColumnsRepository = new TableColumnsRepository(this);

                return _TableColumnsRepository;
            }
        }

        private ReportListsRepository _ReportListsRepository;
        public ReportListsRepository ReportListsRepository
        {
            get
            {

                if (_ReportListsRepository == null)
                    _ReportListsRepository = new ReportListsRepository(this);

                return _ReportListsRepository;
            }
        }

        private ReportListDataTypesRepository _ReportListDataTypesRepository;
        public ReportListDataTypesRepository ReportListDataTypesRepository
        {
            get
            {

                if (_ReportListDataTypesRepository == null)
                    _ReportListDataTypesRepository = new ReportListDataTypesRepository(this);

                return _ReportListDataTypesRepository;
            }
        }

    }
}
