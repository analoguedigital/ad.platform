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
        enableSnapshotView: boolean;

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
        showMap: boolean;
        showPieChart: boolean;
        showTimeline: boolean;
        enableSnapshotView: boolean = true;
        totalFormTemplates: number;
        totalSurveys: number;
        totalImpact: number = 0;

        private TIMELINE_TOGGLE_KEY: string = 'show_timeline';
        private MAP_TOGGLE_KEY: string = 'show_map';
        private PIE_CHART_TOGGLE_KEY: string = 'show_pie_chart';

        static $inject: string[] = ["$http", "$scope", "$rootScope", "session", "$state", "$stateParams", "$q", "$location", 
            "projectSummaryPrintSessionResource", "formTemplateResource", "surveyResource", "projectResource", "localStorageService"];
        constructor(
            private $http: ng.IHttpService,
            private $scope: IProjectSummaryPrintScope,
            private $rootScope: ng.IRootScopeService,
            private session: App.Models.IProjectSummaryPrintSession,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $q: ng.IQService,
            private $location: ng.ILocationService,
            private projectSummaryPrintSessionResource: App.Resources.IProjectSummaryPrintSessionResource,
            private formTemplateResource: App.Resources.IFormTemplateResource,
            private surveyResource: App.Resources.ISurveyResource,
            private projectResource: App.Resources.IProjectResource,
            private localStorageService: ng.local.storage.ILocalStorageService) {

            this.activate();
        }

        activate() {
            var self = this;

            var _timeline = this.$location.search().timeline;
            var _locations = this.$location.search().locations;
            var _pieChart = this.$location.search().piechart;

            if (_timeline && _timeline.length) {
                self.showTimeline = _.toLower(_timeline) === 'true' ? true : false;
            }
            else {
                var toggle = this.localStorageService.get(this.TIMELINE_TOGGLE_KEY);
                if (toggle !== null)
                    self.showTimeline = <boolean>toggle;
            }

            if (_locations && _locations.length)
                self.showMap = _.toLower(_locations) === 'true' ? true : false;
            else {
                var toggle = this.localStorageService.get(this.MAP_TOGGLE_KEY);
                if (toggle !== null)
                    self.showMap = <boolean>toggle;
            }

            if (_pieChart && _pieChart.length)
                self.showPieChart = _.toLower(_pieChart) === 'true' ? true : false;
            else {
                var toggle = this.localStorageService.get(this.PIE_CHART_TOGGLE_KEY);
                if (toggle !== null)
                    self.showPieChart = <boolean>toggle;
                else
                    self.showPieChart = false;
            }

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
                this.surveys = _.sortBy(surveys, ['date', 'formTemplateId']);
                this.formTemplates = formTemplates;
                this.uniqFormTemplates = _.uniqBy(this.formTemplates, (t) => { return t.id });

                this.totalFormTemplates = this.uniqFormTemplates.length;
                this.totalSurveys = this.surveys.length;

                _.forEach(this.uniqFormTemplates, (template) => {
                    this.totalImpact += this.getTotalImpact(template);
                });
            });

            this.$rootScope.$on('timeline-in-snapshot-view', () => {
                this.enableSnapshotView = true;
            });

            this.$rootScope.$on('timeline-in-month-view', () => {
                this.enableSnapshotView = false;
            });

            this.$scope.today = new Date();
            this.$scope.months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
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
            this.showTimeline = true;
            this.showMap = true;
            this.showPieChart = true;
        }

        downloadPdf() {
            this.session.$save().then(() => {
                var requestUrl = "/api/projectSummaryPrintSession/downloadPdf/" + this.session.id + "/" + this.showTimeline + "/" + this.showMap + "/" + this.showPieChart;

                this.$http.get(requestUrl, { responseType: 'arraybuffer' }).then((result) => {
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
                this.$http.get("/api/projectSummaryPrintSession/downloadZip/" + this.session.id + "/" + this.showTimeline + "/" + this.showMap + "/" + this.showPieChart, { responseType: 'arraybuffer' }).then((result) => {
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

        toggleSnapshotView() {
            this.enableSnapshotView = !this.enableSnapshotView;
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

    angular.module("app").controller("projectSummaryPrintController", ProjectSummaryPrintController);
}