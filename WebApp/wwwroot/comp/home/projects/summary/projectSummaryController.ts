﻿declare var moment: any;
declare var Chart: any;

module App {
    "use strict";

    interface IProjectSummaryControllerScope extends ng.IScope {
        safeSurveys: Models.ISurvey[];
        displayedSurveys: Models.ISurvey[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        today: Date;
        months: string[];
    }

    interface IProjectSummaryController {
        searchTerm: string;
        startDate: Date;
        endDate: Date;
        startDateCalendar: any;
        endDateCalendar: any;
        locationCount: number;

        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];

        displayedSurveys: Models.ISurvey[];
        selectedTemplates: Models.IFormTemplate[];

        timelineSnapshotView: boolean;

        activate: () => void;
        clearSearch: () => void;
        clearThreads: () => void;
        clearAll: () => void;
        print: () => void;
        selectAll: () => void;
        selectNone: () => void;
        openStartDateCalendar: () => void;
        openEndDateCalendar: () => void;
        search: () => void;
        delete: (id: string) => void;
    }

    class ProjectSummaryController implements IProjectSummaryController {
        searchTerm: string;
        startDate: Date;
        endDate: Date;
        startDateCalendar = { isOpen: false };
        endDateCalendar = { isOpen: false };
        locationCount: number;

        formTemplates: Models.IFormTemplate[] = [];
        surveys: Models.ISurvey[] = [];
        displayedSurveys: Models.ISurvey[] = [];
        selectedTemplates: Models.IFormTemplate[] = [];
        timelineSnapshotView: boolean = true;

        static $inject: string[] = ["$scope", "$rootScope", "$state", "$q", "$stateParams",
            "projectSummaryPrintSessionResource", "projectResource",
            "formTemplateResource", "surveyResource", "project", "toastr"];

        constructor(
            private $scope: IProjectSummaryControllerScope,
            private $rootScope: ng.IRootScopeService,
            private $state: ng.ui.IStateService,
            private $q: ng.IQService,
            private $stateParams: ng.ui.IStateParamsService,
            private projectSummaryPrintSessionResource: Resources.IProjectSummaryPrintSessionResource,
            private projectResource: Resources.IProjectResource,
            private formTemplateResource: Resources.IFormTemplateResource,
            private surveyResource: Resources.ISurveyResource,
            private project: Models.IProject,
            private toastr: any) {

            this.activate();
        }

        activate() {
            this.$scope.title = this.project.name;
            this.$scope.today = new Date();
            this.$scope.months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            this.startDate = moment().add('-1', 'month').toDate();

            this.$rootScope.$on('timeline-in-snapshot-view', () => {
                this.timelineSnapshotView = true;
            });

            this.$rootScope.$on('timeline-in-month-view', () => {
                this.timelineSnapshotView = false;
            });

            this.load();
        }

        load() {
            this.$q.all([
                this.formTemplateResource.query({ projectId: this.project.id }).$promise,
                this.surveyResource.query({ projectId: this.project.id }).$promise
            ]).then((data) => {
                this.formTemplates = data[0];
                this.surveys = data[1];
                this.$scope.safeSurveys = this.surveys;

                _.map(this.formTemplates, (t) => { t.isChecked = true });

                this.formTemplates = _.filter(this.formTemplates, (t) => {
                    return t.projectId == this.project.id;
                });

                // populate data sets
                this.displayedSurveys = this.surveys;
                this.selectedTemplates = this.formTemplates;
                this.$scope.displayedSurveys = this.displayedSurveys;
            });
        }

        findMetricFilter(metricFilter: Models.IMetricFilter, filters: Models.IMetricFilter[]) {
            var result = undefined;
            var count = 0;

            _.forEach(filters, (item) => {
                if (item.shortTitle === metricFilter.shortTitle && item.type === metricFilter.type) {
                    result = item;
                    count++;
                }
            });

            if (result && count > 1)
                return result;

            return undefined;
        }

        clearSearch() {
            this.searchTerm = undefined;
            this.startDate = undefined;
            this.endDate = undefined;
        }

        clearThreads() {
            angular.forEach(this.formTemplates, (t) => { t.isChecked = false });
        }

        clearFilterValues() {
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
                        _.forEach(multipleValue.options, (opt) => { opt.selected = false });
                        break;
                    }
                }
            });
        }

        clearAll() {
            this.searchTerm = undefined;
            this.startDate = undefined;
            this.endDate = undefined;

            this.clearFilterValues();
            this.load();
        }

        print() {
            let surveys = _.filter(this.surveys, (r) => { return r.isChecked });
            if (surveys.length < 1) {
                this.toastr.info('Select desired records first', 'Print Surveys');
                return false;
            }

            let printSession = <Models.IProjectSummaryPrintSession>{};
            printSession.projectId = this.project.id;
            printSession.surveyIds = _.map(surveys, (survey) => { return survey.id; });

            this.projectSummaryPrintSessionResource.save(printSession).$promise.then((session) => {
                this.$state.go("home.projects.summaryPrint", { sessionId: session.id });
            });
        }

        selectAll() {
            _.forEach(this.displayedSurveys, (survey) => {
                survey.isChecked = true;
            });
        }

        selectNone() {
            _.forEach(this.displayedSurveys, (survey) => {
                survey.isChecked = false;
            });
        }

        getFormTemplate(id: string) {
            let template = _.find(this.formTemplates, (t) => { return t.id == id; });
            if (template && template.title)
                return template.title;

            return '';
        }

        getTemplateColour(id: string) {
            let template = _.find(this.formTemplates, (t) => { return t.id == id; });
            if (template && template.colour && template.colour.length)
                return template.colour;

            return '';
        }

        getThreadsCount() {
            let templates = _.uniq(_.map(this.displayedSurveys, (r) => { return r.formTemplateId }));
            return templates.length;
        }

        openStartDateCalendar() {
            this.startDateCalendar.isOpen = true;
        }

        openEndDateCalendar() {
            this.endDateCalendar.isOpen = true;
        }

        timelineNextMonth() {
            this.$scope.today = moment(this.$scope.today).add(1, 'months').toDate();
            this.$rootScope.$broadcast('timeline-next-month');
        }

        timelinePreviousMonth() {
            this.$scope.today = moment(this.$scope.today).subtract(1, 'months').toDate();
            this.$rootScope.$broadcast('timeline-previous-month');
        }

        delete(id: string) {
            this.surveyResource.delete({ id: id },
                () => { this.load(); },
                (err) => { console.error(err); });
        }

        search() {
            var templateIds = _.map(_.filter(this.formTemplates, (template) => { return template.isChecked == true }), (template) => { return template.id });

            var searchModel: Models.SearchDTO = {
                projectId: this.project.id,
                formTemplateIds: templateIds,
                term: this.searchTerm,
                startDate: this.startDate,
                endDate: this.endDate,
                filterValues: []
            };

            this.surveyResource.search(searchModel, (surveys: Models.ISurvey[]) => {
                this.surveys = surveys;
                this.displayedSurveys = [].concat(this.surveys);
                this.$scope.displayedSurveys = this.displayedSurveys;

                if (templateIds.length < 1)
                    this.selectedTemplates = [];
            }, (error) => {
                console.error(error);
            });
        }
    }

    angular.module("app").controller("projectSummaryController", ProjectSummaryController);

}   