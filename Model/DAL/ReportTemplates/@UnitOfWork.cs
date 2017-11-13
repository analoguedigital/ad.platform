using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMethods.Survey.Models.DAL
{
    public partial class UnitOfWork
    {
        private ReportsRepository _ReportsRepository;
        public ReportsRepository ReportsRepository
        {
            get
            {

                if (this._ReportsRepository == null)
                {
                    this._ReportsRepository = new ReportsRepository(this);
                }
                return _ReportsRepository;
            }
        }

        private ReportTemplateCategoriesRepository _ReportTemplateCategoriesRepository;
        public ReportTemplateCategoriesRepository ReportTemplateCategoriesRepository
        {
            get
            {

                if (this._ReportTemplateCategoriesRepository == null)
                {
                    this._ReportTemplateCategoriesRepository = new ReportTemplateCategoriesRepository(this);
                }
                return _ReportTemplateCategoriesRepository;
            }
        }

        private ReportTemplatesRepository _ReportTemplatesRepository;
        public ReportTemplatesRepository ReportTemplatesRepository
        {
            get
            {

                if (this._ReportTemplatesRepository == null)
                {
                    this._ReportTemplatesRepository = new ReportTemplatesRepository(this);
                }
                return _ReportTemplatesRepository;
            }
        }

        private ReportChartsRepository _ReportChartsRepository;
        public ReportChartsRepository ReportChartsRepository
        {
            get
            {

                if (this._ReportChartsRepository == null)
                {
                    this._ReportChartsRepository = new ReportChartsRepository(this);
                }
                return _ReportChartsRepository;
            }
        }

        private ChartSerieTypesRepository _ChartSerieTypesRepository;
        public ChartSerieTypesRepository ChartSerieTypesRepository
        {
            get
            {

                if (this._ChartSerieTypesRepository == null)
                {
                    this._ChartSerieTypesRepository = new ChartSerieTypesRepository(this);
                }
                return _ChartSerieTypesRepository;
            }
        }

        private ChartSeriesRepository _ChartSeriesRepository;
        public ChartSeriesRepository ChartSeriesRepository
        {
            get
            {

                if (this._ChartSeriesRepository == null)
                {
                    this._ChartSeriesRepository = new ChartSeriesRepository(this);
                }
                return _ChartSeriesRepository;
            }
        }

        private ReportTablesRepository _ReportTablesRepository;
        public ReportTablesRepository ReportTablesRepository
        {
            get
            {

                if (this._ReportTablesRepository == null)
                {
                    this._ReportTablesRepository = new ReportTablesRepository(this);
                }
                return _ReportTablesRepository;
            }
        }

        private TableColumnsRepository _TableColumnsRepository;
        public TableColumnsRepository TableColumnsRepository
        {
            get
            {

                if (this._TableColumnsRepository == null)
                {
                    this._TableColumnsRepository = new TableColumnsRepository(this);
                }
                return _TableColumnsRepository;
            }
        }

        private ReportListsRepository _ReportListsRepository;
        public ReportListsRepository ReportListsRepository
        {
            get
            {

                if (this._ReportListsRepository == null)
                {
                    this._ReportListsRepository = new ReportListsRepository(this);
                }
                return _ReportListsRepository;
            }
        }

        private ReportListDataTypesRepository _ReportListDataTypesRepository;
        public ReportListDataTypesRepository ReportListDataTypesRepository
        {
            get
            {

                if (this._ReportListDataTypesRepository == null)
                {
                    this._ReportListDataTypesRepository = new ReportListDataTypesRepository(this);
                }
                return _ReportListDataTypesRepository;
            }
        }

    }
}
