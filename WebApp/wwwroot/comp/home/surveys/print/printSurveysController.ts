module App {
    "use strict";

    export interface IPrintSurveysScope extends ng.IScope {
        ctrl: IPrintSurveysController;
    }

    interface IPrintSurveysController {
        title: string;
        formTemplates: Array<Models.IFormTemplate>;
        surveys: Array<Models.ISurvey>;
        activate: () => void;
    }

    class PrintSurveysController implements IPrintSurveysController {
        title: string = "Forms";
        formTemplates: Array<Models.IFormTemplate> = [];
        surveys: Array<Models.ISurvey> = [];

        static $inject: string[] = ["$state", "$stateParams", "$q", "formTemplateResource", "surveyResource"];
        constructor(
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $q: ng.IQService,
            private formTemplateResource: App.Resources.IFormTemplateResource,
            private surveyResource: App.Resources.ISurveyResource) {

            this.activate();
        }

        activate() {
            let formTemplates = [];
            let surveys = [];
            let promises = [];

            angular.forEach(this.$stateParams.selectedSurveys, (surveyId) => {
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
            });
        }

        getFormTemplate(formTemplateId: string) {
            return _.find(this.formTemplates, { "id": formTemplateId });
        }

        deleteMetric(metric: Models.IMetric) {
            metric.isDeleted = true;
        }
        
        deleteGroup(group: Models.IMetricGroup) {
            group.isDeleted = true;
        }

        resetView() {
            angular.forEach(this.formTemplates, (formTemplate) => {
                angular.forEach(formTemplate.metricGroups, (metricGroup) => {
                    metricGroup.isDeleted = false;
                    angular.forEach(metricGroup.metrics, (metric) => {
                        metric.isDeleted = false;
                    });
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

    angular.module("app").controller("printSurveysController", PrintSurveysController);
}