module App {
    "use strict";

    export interface IProjectSummaryPrintScope extends ng.IScope {
        ctrl: IProjectSummaryPrintController;
    }

    interface IProjectSummaryPrintController {
        title: string;
        project: Models.IProject;
        formTemplates: Array<Models.IFormTemplate>;
        surveys: Array<Models.ISurvey>;
        uniqFormTemplates: Models.IFormTemplate[];
        locationCount: number;

        activate: () => void;
        downloadPdf: () => void;
        downloadZip: () => void;

        showMap: boolean;
        showPieChart: boolean;
        showTimeline: boolean;

        totalFormTemplates: number;
        totalSurveys: number;
        totalImpact: number;
    }

    class ProjectSummaryPrintController implements IProjectSummaryPrintController {
        title: string = "Forms";
        project: Models.IProject;
        formTemplates: Array<Models.IFormTemplate> = [];
        surveys: Array<Models.ISurvey> = [];
        uniqFormTemplates: Models.IFormTemplate[];
        locationCount: number;
        showMap: boolean = false;
        showPieChart: boolean = false;
        showTimeline: boolean = false;
        totalFormTemplates: number;
        totalSurveys: number;
        totalImpact: number = 0;

        private TIMELINE_TOGGLE_KEY: string = 'show_timeline';
        private MAP_TOGGLE_KEY: string = 'show_map';
        private PIE_CHART_TOGGLE_KEY: string = 'show_pie_chart';

        static $inject: string[] = ["$http", "session", "$state", "$stateParams", "$q",
            "projectSummaryPrintSessionResource", "formTemplateResource", "surveyResource", "projectResource", "localStorageService"];
        constructor(
            private $http: ng.IHttpService,
            private session: App.Models.IProjectSummaryPrintSession,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $q: ng.IQService,
            private projectSummaryPrintSessionResource: App.Resources.IProjectSummaryPrintSessionResource,
            private formTemplateResource: App.Resources.IFormTemplateResource,
            private surveyResource: App.Resources.ISurveyResource,
            private projectResource: App.Resources.IProjectResource,
            private localStorageService: ng.local.storage.ILocalStorageService) {

            // read chart toggles
            var timelineToggle = this.localStorageService.get(this.TIMELINE_TOGGLE_KEY);
            if (timelineToggle !== null)
                this.showTimeline = <boolean>timelineToggle;

            var mapToggle = this.localStorageService.get(this.MAP_TOGGLE_KEY);
            if (mapToggle !== null)
                this.showMap = <boolean>mapToggle;

            var pieChartToggle = this.localStorageService.get(this.PIE_CHART_TOGGLE_KEY);
            if (pieChartToggle !== null)
                this.showPieChart = <boolean>pieChartToggle;

            this.activate();
        }

        activate() {
            let formTemplates = [];
            let surveys = [];
            let promises = [];

            angular.forEach(this.session.surveyIds, (surveyId) => {
                let deferred = this.$q.defer();
                promises.push(deferred.promise);

                this.surveyResource.get({ id: surveyId }).$promise
                    .then((survey) => {
                        surveys.push(survey);

                        this.formTemplateResource.get({ id: survey.formTemplateId }).$promise
                            .then((data) => {
                                formTemplates.push(data);
                                deferred.resolve();
                            });
                    });
            });

            let deferred = this.$q.defer();
            promises.push(deferred);

            this.projectResource.get({ id: this.session.projectId }).$promise
                .then((data) => {
                    this.project = data;
                    deferred.resolve();
                });

            this.$q.all(promises).then(() => {
                this.surveys = surveys;
                this.formTemplates = formTemplates;
                this.uniqFormTemplates = _.uniqBy(this.formTemplates, (t) => { return t.id });

                this.totalFormTemplates = this.uniqFormTemplates.length;
                this.totalSurveys = this.surveys.length;

                _.forEach(this.uniqFormTemplates, (template) => {
                    this.totalImpact += this.getTotalImpact(template);
                });
            });
        }

        getTotalImpact(template: Models.IFormTemplate) {
            var totalValue = 0;

            if (template.timelineBarMetricId && template.timelineBarMetricId.length) {
                var records = _.filter(this.surveys, (s) => { return s.formTemplateId == template.id; });
                _.forEach(records, (r) => {
                    var impactValues = _.filter(r.formValues, (fv) => { return fv.metricId == template.timelineBarMetricId; });
                    if (impactValues.length) {
                        totalValue += impactValues[0].numericValue;
                    }
                });
            }

            return totalValue;
        }

        getSurveyImpact(survey: Models.ISurvey) {
            var impactValue = 0;

            var templates = _.filter(this.uniqFormTemplates, (t) => { return t.id == survey.formTemplateId; });
            if (templates && templates.length) {
                var template = templates[0];
                if (template.timelineBarMetricId && template.timelineBarMetricId.length) {
                    var formValues = _.filter(survey.formValues, (fv) => { return fv.metricId == template.timelineBarMetricId; });
                    if (formValues && formValues.length) {
                        impactValue = formValues[0].numericValue;
                    }
                }
            }

            return impactValue;
        }

        getFormTemplate(formTemplateId: string) {
            return _.find(this.formTemplates, { "id": formTemplateId });
        }

        deleteItem(id: string) {
            this.session.removedItemIds.push(id);
        }

        isItemVisible(itemId: string) {
            return this.session.removedItemIds.indexOf(itemId) === -1;
        }

        resetView() {
            this.session.removedItemIds = [];
        }

        downloadPdf() {
            this.session.$save().then(() => {
                this.$http.get("/api/projectSummaryPrintSession/downloadPdf/" + this.session.id, { responseType: 'arraybuffer' }).then((result) => {
                    let headers = result.headers();

                    var filename = headers['x-filename'];
                    var contentType = headers['content-type'];

                    var linkElement = document.createElement('a');
                    try {
                        var blob = new Blob([result.data], { type: contentType });
                        var url = window.URL.createObjectURL(blob);

                        linkElement.setAttribute('href', url);
                        linkElement.setAttribute("download", filename);

                        var clickEvent = new MouseEvent("click", {
                            "view": window,
                            "bubbles": true,
                            "cancelable": false
                        });
                        linkElement.dispatchEvent(clickEvent);
                    } catch (ex) {
                        console.log(ex);
                    }
                });
            });
        }

        downloadZip() {
            this.session.$save().then(() => {
                this.$http.get("/api/projectSummaryPrintSession/downloadZip/" + this.session.id, { responseType: 'arraybuffer' }).then((result) => {
                    let headers = result.headers();

                    var filename = headers['x-filename'];
                    var contentType = headers['content-type'];

                    var linkElement = document.createElement('a');
                    try {
                        var blob = new Blob([result.data], { type: contentType });
                        var url = window.URL.createObjectURL(blob);

                        linkElement.setAttribute('href', url);
                        linkElement.setAttribute("download", filename);

                        var clickEvent = new MouseEvent("click", {
                            "view": window,
                            "bubbles": true,
                            "cancelable": false
                        });
                        linkElement.dispatchEvent(clickEvent);
                    } catch (ex) {
                        console.log(ex);
                    }
                });
            });
        }

        addFormValue(metric, rowDataListItem, rowNumber) {
            var formValue = <Models.IFormValue>{};
            formValue.textValue = '';
            formValue.metricId = metric.id;
            formValue.rowNumber = rowNumber;
            if (rowDataListItem)
                formValue.rowDataListItemId = rowDataListItem.id;

            // no need to add the new formValue to the surveys as we are in the print mode 
            return formValue;
        }

        toggleTimeline() {
            this.showTimeline = !this.showTimeline;
            this.localStorageService.set(this.TIMELINE_TOGGLE_KEY, this.showTimeline);
        }

        toggleMap() {
            this.showMap = !this.showMap;
            this.localStorageService.set(this.MAP_TOGGLE_KEY, this.showMap);
        }

        togglePieChart() {
            this.showPieChart = !this.showPieChart;
            this.localStorageService.set(this.PIE_CHART_TOGGLE_KEY, this.showPieChart);
        }
    }

    angular.module("app").controller("projectSummaryPrintController", ProjectSummaryPrintController);
}