module App {
    "use strict";

    interface IAllSurveysControllerScope extends ng.IScope {
        filterValues: Models.IFilterValue[];
    }

    interface IAllSurveysController {
        title: string;
        searchTerm: string;
        surveys: Models.ISurvey[];
        surveysData: string[];
        surveysDataHeaders: string[];
        displayedSurveys: Models.ISurvey[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
        isDataView: boolean;
        startDate: Date;
        endDate: Date;
        startDateCalendar: any;
        endDateCalendar: any;
        metricFilters: Models.IMetricFilter[];

        activate: () => void;
        delete: (id: string) => void;
        currentUser: Models.IOrgUser;
        assignment: Models.IProjectAssignment;
        search: () => void;
        resetSearch: () => void;
        openStartDateCalendar: () => void;
        openEndDateCalendar: () => void;
        getAttachmentsCount: (survey: Models.ISurvey) => number;
    }

    class AllSurveysController implements IAllSurveysController {
        title: string;
        searchTerm: string;
        surveys: Models.ISurvey[];
        surveysData: string[];
        displayedSurveysData: string[];
        surveysDataHeaders: string[];
        displayedSurveys: Models.ISurvey[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
        isDataView: boolean;
        startDate: Date;
        endDate: Date;
        currentUser: Models.IOrgUser;
        assignment: Models.IProjectAssignment;
        startDateCalendar: any;
        endDateCalendar: any;
        isAdvSearchOpen: boolean;
        metricFilters: Models.IMetricFilter[];

        static $inject: string[] = ["$scope", "$rootScope", "$timeout", "project", "formTemplate", "formTemplateResource", "surveyResource", "dataResource", "userContextService"];
        constructor(
            private $scope: IAllSurveysControllerScope,
            private $rootScope: ng.IRootScopeService,
            private $timeout: ng.ITimeoutService,
            public project: Models.IProject,
            private formTemplate: Models.IFormTemplate,
            private formTemplateResource: Resources.IFormTemplateResource,
            private surveyResource: Resources.ISurveyResource,
            private dateResource: Resources.IDataResource,
            private userContextService: Services.IUserContextService) {

            this.title = "AllSurveys";
            this.isDataView = false;
            this.activate();
        }

        activate() {
            this.startDateCalendar = { isOpen: false };
            this.endDateCalendar = { isOpen: false };

            this.$scope.$on('$viewContentLoaded', () => {
                this.$timeout(() => {
                    this.$scope.$broadcast('rzSliderForceRender');
                }, 500);
            });

            this.metricFilters = [];
            this.$scope.filterValues = [];

            this.load();
        }

        openStartDateCalendar() {
            this.startDateCalendar.isOpen = true;
        }

        openEndDateCalendar() {
            this.endDateCalendar.isOpen = true;
        }

        load() {
            this.formTemplateResource.getFilters({ id: this.formTemplate.id }, (filters) => {
                this.metricFilters = filters;
            }, (error) => {
                console.error(error);
            });

            this.surveyResource.query({ projectId: this.project.id }).$promise.then((surveys) => {
                this.surveys = _.filter(surveys, { formTemplateId: this.formTemplate.id });
                this.displayedSurveys = [].concat(this.surveys);

                this.dateResource.query({ projectId: this.project.id, formTemplateId: this.formTemplate.id }).$promise.then((dataRows) => {
                    this.surveysDataHeaders = dataRows[0];
                    this.surveysData = dataRows.slice(1);
                    this.displayedSurveysData = [].concat(this.surveysData);
                });
            });
        }

        delete(id: string) {
            this.surveyResource.delete({ id: id },
                () => { this.load(); },
                (err) => { console.log(err); });
        }

        toggleAdvancedSearch() {
            this.isAdvSearchOpen = !this.isAdvSearchOpen;

            if (this.isAdvSearchOpen) {
                this.$timeout(() => {
                    this.$scope.$broadcast('rzSliderForceRender');
                }, 100);
            }
        }

        resetSearch() {
            this.searchTerm = undefined;
            this.startDate = undefined;
            this.endDate = undefined;

            _.forEach(this.$scope.filterValues, (fv) => {
                switch (fv.type) {
                    case "single": {
                        var singleValue = <Models.ISingleFilterValue>fv;
                        singleValue.value = undefined;

                        break;
                    }
                    case "range": {
                        var rangeValue = <Models.IRangeFilterValue>fv;

                        rangeValue.fromValue = undefined;
                        rangeValue.toValue = undefined;

                        break;
                    }
                    case "multiple": {
                        var multipleValue = <Models.IMultipleFilterValue>fv;
                        multipleValue.values = [];
                        _.forEach(multipleValue.options, (opt) => { opt.selected = true });

                        break;
                    }
                }
            });

            this.load();
        }

        getFilterValues() {
            var filterValues = [];

            _.forEach(this.$scope.filterValues, (filterValue) => {
                switch (filterValue.type) {
                    case "single": {
                        var singleValue = <Models.ISingleFilterValue>filterValue;
                        if (singleValue.value && singleValue.value.length)
                            filterValues.push(filterValue);

                        break;
                    }
                    case "range": {
                        var rangeValue = <Models.IRangeFilterValue>filterValue;
                        var fromValue = rangeValue.fromValue;
                        var toValue = rangeValue.toValue;

                        if (fromValue || toValue)
                            filterValues.push(filterValue);

                        break;
                    }
                    case "multiple": {
                        var multipleValue = <Models.IMultipleFilterValue>filterValue;
                        if (multipleValue.values && multipleValue.values.length)
                            filterValues.push(multipleValue);

                        break;
                    }
                }
            });

            return filterValues;
        }

        search() {
            var filterValues = this.getFilterValues();

            var searchModel: Models.SearchDTO = {
                projectId: this.project.id,
                formTemplateIds: [this.formTemplate.id],
                term: this.searchTerm,
                startDate: this.startDate,
                endDate: this.endDate,
                filterValues: filterValues
            };

            this.surveyResource.search(searchModel, (surveys: Models.ISurvey[]) => {
                this.surveys = _.filter(surveys, { formTemplateId: this.formTemplate.id });
                this.displayedSurveys = [].concat(this.surveys);
            }, (error) => {
                console.error(error);
            });
        }

        getDescriptionHeading() {
            if (this.formTemplate.descriptionFormat.length) {
                var metricTitles = [];
                let descFormat = this.formTemplate.descriptionFormat;
                var pattern = /{{\s*([^}]+)\s*}}/g;
                var segment;

                while (segment = pattern.exec(descFormat))
                    metricTitles.push(segment[1]);

                return metricTitles.join(' - ');
            }

            return "Your record";
        }

        getAttachmentsCount(survey: Models.ISurvey) {
            var attachmentCount = 0;
            _.forEach(survey.formValues, (fv) => {
                if (fv.attachments.length > 0)
                    attachmentCount += fv.attachments.length;
            });

            return attachmentCount;
        }

        hasAccess(template: Models.IFormTemplate, flag: string): boolean {
            var authorized = false;

            switch (flag) {
                case 'view': {
                    authorized = template.canView === null ? this.project.allowView : template.canView;
                    break;
                }
                case 'add': {
                    authorized = template.canAdd === null ? this.project.allowAdd : template.canAdd;
                    break;
                }
                case 'edit': {
                    authorized = template.canEdit === null ? this.project.allowEdit : template.canEdit;
                    break;
                }
                case 'delete': {
                    authorized = template.canDelete === null ? this.project.allowDelete : template.canDelete;
                    break;
                }
            }

            return authorized;
        }
    }

    angular.module("app").controller("allSurveysController", AllSurveysController);
}