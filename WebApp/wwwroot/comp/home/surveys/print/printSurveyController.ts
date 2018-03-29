module App {
    "use strict";

    export interface IPrintSurveyScope extends ng.IScope {
        ctrl: IPrintSurveyController;
    }

    interface IPrintSurveyController {
        title: string;
        activate: () => void;
    }

    class PrintSurveyController implements IPrintSurveyController {
        title: string = "Forms";

        static $inject: string[] = ["$state", "formTemplateResource", "surveyResource", "formTemplate", "survey"];
        constructor(
            private $state: ng.ui.IStateService,
            private formTemplateResource: App.Resources.IFormTemplateResource,
            private surveyResource: App.Resources.ISurveyResource,
            private formTemplate: Models.IFormTemplate,
            private survey: Models.ISurvey) {

            this.activate();
        }

        activate() { }

        deleteMetric(metric: Models.IMetric) {
            metric.isDeleted = true;
        }

        deleteGroup(group: Models.IMetricGroup) {
            group.isDeleted = true;
        }

        resetView() {
            angular.forEach(this.formTemplate.metricGroups, (metricGroup) => {
                metricGroup.isDeleted = false;
                angular.forEach(metricGroup.metrics, (metric) => {
                    metric.isDeleted = false;
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

            this.survey.formValues.push(formValue);

            return formValue;
        }

    }

    angular.module("app").controller("printSurveyController", PrintSurveyController);
}