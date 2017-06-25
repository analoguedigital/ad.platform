module App {
    "use strict";

    interface IProjectSummaryController {
        searchTerm: string;
        startDate: Date;
        endDate: Date;
        startDateCalendar: any;
        endDateCalendar: any;
        locationCount: number;

        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        threads: Models.IFormTemplate[];
        records: Models.ISurvey[];

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
        threads: Models.IFormTemplate[] = [];
        records: Models.ISurvey[] = [];

        static $inject: string[] = ["$scope", "$state", "$q", "$stateParams", "projectSummaryPrintSessionResource", "projectResource", "formTemplateResource", "surveyResource", "project", "toastr"];

        constructor(
            private $scope: ng.IScope,
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

                this.applyFilters();
            });
        }

        applyFilters() {
            // the recordings table is bound to this collection
            this.records = [];

            // select checked threads
            this.threads = _.filter(this.formTemplates, (t) => { return t.isChecked == true });

            // extract thread surveys
            let surveys = [];
            angular.forEach(this.threads, (t) => {
                let filledForms = _.filter(this.surveys, (s) => { return s.formTemplateId == t.id });
                angular.forEach(filledForms, (form) => { surveys.push(form); });
            });

            // apply filters and collect surveys
            let collectedRecords = [];
            let resultRecords = [];

            // search term
            if (this.searchTerm == undefined || this.searchTerm.length < 1) {
                collectedRecords = surveys;
            } else {
                _.forEach(surveys, (survey: Models.ISurvey) => {
                    if (_.includes(survey.serial.toString(), this.searchTerm))
                        collectedRecords.push(survey);

                    if (_.includes(survey.description, this.searchTerm))
                        collectedRecords.push(survey);

                    _.forEach(survey.formValues, (fm) => {
                        if (fm.textValue && fm.textValue.length) {
                            if (_.includes(fm.textValue, this.searchTerm)) {
                                collectedRecords.push(survey);
                            }
                        }
                    });
                });
            }

            // date range filter
            angular.forEach(collectedRecords, (survey: Models.ISurvey, index) => {
                // has start date
                let _surveyDate = survey.surveyDate.setHours(0, 0, 0, 0);

                if (this.startDate == undefined && this.endDate == undefined) {
                    resultRecords.push(survey);
                }

                if (this.startDate && this.endDate == undefined) {
                    if (_surveyDate >= this.startDate.setHours(0, 0, 0, 0)) {
                        resultRecords.push(survey);
                    }
                }

                // has end date
                if (this.endDate && this.startDate == undefined) {
                    if (_surveyDate <= this.endDate.setHours(0, 0, 0, 0))
                        resultRecords.push(survey);
                }

                // has date range
                if (this.startDate && this.endDate) {
                    if (_surveyDate >= this.startDate.setHours(0, 0, 0, 0) && _surveyDate <= this.endDate.setHours(0, 0, 0, 0))
                        resultRecords.push(survey);
                }
            });

            // this will be the displayed record
            this.records = _.uniqBy(resultRecords, (rec: Models.ISurvey) => { return rec.id });
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

            this.threads = [];
            this.records = [];
        }

        print() {
            let surveys = _.filter(this.records, (r) => { return r.isChecked });
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
            _.map(this.records, (r) => { r.isChecked = true; });
        }

        selectNone() {
            _.map(this.records, (r) => { r.isChecked = false; });
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
            let templates = _.uniq(_.map(this.records, (r) => { return r.formTemplateId }));
            return templates.length;
        }

        openStartDateCalendar() {
            this.startDateCalendar.isOpen = true;
        }

        openEndDateCalendar() {
            this.endDateCalendar.isOpen = true;
        }

    }

    angular.module("app").controller("projectSummaryController", ProjectSummaryController);

}   