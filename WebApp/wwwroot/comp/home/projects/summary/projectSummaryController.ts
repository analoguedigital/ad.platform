declare var moment: any;
declare var Chart: any;

module App {
    "use strict";

    interface IProjectSummaryControllerScope extends ng.IScope {
        tableSearchTerm: string;
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
        timelineSnapshotView: boolean;

        static $inject: string[] = ["$scope", "$rootScope", "$state", "$q", "$stateParams",
            "projectSummaryPrintSessionResource", "projectResource",
            "formTemplateResource", "surveyResource", "project",
            "toastr", "projectSummaryService"];

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
            private toastr: any,
            private projectSummaryService: Services.IProjectSummaryService) {

            this.activate();
        }

        activate() {
            this.$scope.title = this.project.name;
            this.$scope.today = new Date();
            this.$scope.months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

            this.$rootScope.$on('timeline-in-snapshot-view', () => {
                this.timelineSnapshotView = true;
            });

            this.$rootScope.$on('timeline-in-month-view', () => {
                this.timelineSnapshotView = false;
            });

            this.bindWatchers();
            this.load();
        }

        bindWatchers() {
            this.$scope.$watch('ctrl.searchTerm', _.debounce((newValue, oldValue) => {
                this.$scope.$apply(() => {
                    if ((oldValue == undefined && newValue) || (oldValue && newValue.length < 1) || (oldValue && oldValue.length && newValue.length)) {
                        this.applyFilters();
                    }
                });
            }, 1000));

            this.$scope.$watch('ctrl.startDate', _.debounce((newValue, oldValue) => {
                this.$scope.$apply(() => {
                    if (this.startDate || (oldValue && newValue == null)) {
                        this.applyFilters();
                    }
                });
            }, 1000));

            this.$scope.$watch('ctrl.endDate', _.debounce((newValue, oldValue) => {
                this.$scope.$apply(() => {
                    if (this.endDate || (oldValue && newValue == null)) {
                        this.applyFilters();
                    }
                });
            }, 1000));
        }

        load() {
            this.$q.all([
                this.formTemplateResource.query({ projectId: this.project.id }).$promise,
                this.surveyResource.query({ projectId: this.project.id }).$promise
            ]).then((data) => {
                this.formTemplates = data[0];
                this.surveys = data[1];

                _.map(this.formTemplates, (t) => { t.isChecked = true });

                this.projectSummaryService.formTemplates = this.formTemplates;
                this.projectSummaryService.surveys = this.surveys;

                this.$scope.safeSurveys = this.surveys;

                this.applyFilters();
            });
        }

        applyFilters() {
            this.projectSummaryService.query = this.searchTerm;
            this.projectSummaryService.fromDate = this.startDate;
            this.projectSummaryService.toDate = this.endDate;

            this.projectSummaryService.filter().then((result: Services.IProjectSummaryServiceResultView) => {
                this.displayedSurveys = result.surveys;
                this.selectedTemplates = result.templates;

                this.$scope.displayedSurveys = this.displayedSurveys;
            });
        }

        threadsChanged() {
            this.applyFilters();
        }

        clearSearch() {
            this.searchTerm = '';
            this.startDate = undefined;
            this.endDate = undefined;
        }

        clearThreads() {
            angular.forEach(this.formTemplates, (t) => { t.isChecked = false });
            this.applyFilters();
        }

        clearAll() {
            this.searchTerm = '';
            this.startDate = undefined;
            this.endDate = undefined;
            angular.forEach(this.formTemplates, (t) => { t.isChecked = false });
        }

        print() {
            let surveys = _.filter(this.displayedSurveys, (r) => { return r.isChecked });
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

    }

    angular.module("app").controller("projectSummaryController", ProjectSummaryController);

}   