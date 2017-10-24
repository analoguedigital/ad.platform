module App {
    "use strict";

    export interface IProjectSummaryPrintScope extends ng.IScope {
        ctrl: IProjectSummaryPrintController;
    }

    interface IProjectSummaryPrintController {
        title: string;
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
        formTemplates: Array<Models.IFormTemplate> = [];
        surveys: Array<Models.ISurvey> = [];
        uniqFormTemplates: Models.IFormTemplate[];
        locationCount: number;
        showMap: boolean = true;
        showPieChart: boolean = true;
        showTimeline: boolean = true;
        totalFormTemplates: number;
        totalSurveys: number;
        totalImpact: number = 0;

        static $inject: string[] = ["$http", "session", "$state", "$stateParams", "$q", "projectSummaryPrintSessionResource", "formTemplateResource", "surveyResource"];
        constructor(
            private $http: ng.IHttpService,
            private session: App.Models.IProjectSummaryPrintSession,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $q: ng.IQService,
            private projectSummaryPrintSessionResource: App.Resources.IProjectSummaryPrintSessionResource,
            private formTemplateResource: App.Resources.IFormTemplateResource,
            private surveyResource: App.Resources.ISurveyResource) {

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
    }

    angular.module("app").controller("projectSummaryPrintController", ProjectSummaryPrintController);
}