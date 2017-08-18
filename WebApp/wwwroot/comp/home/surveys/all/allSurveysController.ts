
module App {
    "use strict";

    interface IAllSurveysControllerScope extends ng.IScope {
        metricFilters: any[];
        filterValues: any[];
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

        activate: () => void;
        delete: (id: string) => void;
        search: () => void;
        resetSearch: () => void;
        openStartDateCalendar: () => void;
        openEndDateCalendar: () => void;
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
        startDateCalendar: any;
        endDateCalendar: any;

        isAdvSearchOpen: boolean;

        static $inject: string[] = ["$scope", "$timeout", "project", "formTemplate", "surveyResource", "dataResource"];
        constructor(
            private $scope: IAllSurveysControllerScope,
            private $timeout: ng.ITimeoutService,
            public project: Models.IProject,
            private formTemplate: Models.IFormTemplate,
            private surveyResource: Resources.ISurveyResource,
            private dateResource: Resources.IDataResource) {

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

            this.$scope.metricFilters = [];
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
            _.forEach(this.$scope.filterValues, (fv) => {
                switch (fv.type) {
                    case "single": {
                        fv.value = undefined;
                        break;
                    }
                    case "range": {
                        fv.fromValue = undefined;
                        fv.toValue = undefined;
                        break;
                    }
                    case "multiple": {
                        fv.values = [];
                        break;
                    }
                }
            });

            this.$scope.$broadcast('reset-filter-controls');

            this.load();
        }

        getFilterValues() {
            var filterValues = [];

            _.forEach(this.$scope.filterValues, (filterValue) => {
                switch (filterValue.type) {
                    case "single": {
                        if (filterValue.value && filterValue.value.length)
                            filterValues.push(filterValue);

                        break;
                    }
                    case "range": {
                        var fromValue = filterValue.fromValue;
                        var toValue = filterValue.toValue;

                        if (fromValue || toValue)
                            filterValues.push(filterValue);

                        break;
                    }
                    case "multiple": {
                        if (filterValue.values && filterValue.values.length)
                            filterValues.push(filterValue);

                        break;
                    }
                }
            });

            return filterValues;
        }

        search() {
            var filterValues = this.getFilterValues();

            var model = {
                projectId: this.project.id,
                formTemplateId: this.formTemplate.id,
                term: this.searchTerm,
                startDate: this.startDate,
                endDate: this.endDate,
                filterValues: filterValues
            };

            this.surveyResource.search(model, (surveys: Models.ISurvey[]) => {
                this.surveys = _.filter(surveys, { formTemplateId: this.formTemplate.id });
                this.displayedSurveys = [].concat(this.surveys);
            }, (error) => {
                console.error(error);
            });
        }
    }

    angular.module("app").controller("allSurveysController", AllSurveysController);
}