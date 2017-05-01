module App {
    "use strict";

    interface IMetricController {
        activate: () => void;
    }

    export interface IMetricControllerScope extends INewSurveyScope {
        metric: Models.IMetric;
        metricGroup: Models.IMetricGroup;
        inputName: string;
        inputErrorsKey: string;
        viewName: string;
        isCompact: boolean;
        rowNumber: number;
        dataListItem: Models.IDataListItem;
        formValues: Models.IFormValue[];
        formValue: Models.IFormValue;
    }

    class MetricController implements IMetricController {

        static $inject: string[] = ["$scope"];

        constructor(private $scope: IMetricControllerScope) {

            this.activate();
            $scope.$watch('metric', () => { this.activate(); });
        }

        activate() {

            var relatedFormValues: Models.IFormValue[] = [];

            if (this.$scope.metricGroup.isRepeater) {

                if (this.$scope.metricGroup.type === "IterativeRepeater") {
                    relatedFormValues = _.filter(this.$scope.ctrl.survey.formValues, { 'metricId': this.$scope.metric.id, 'rowNumber': this.$scope.rowNumber });
                } else {
                    relatedFormValues = _.filter(this.$scope.ctrl.survey.formValues, { 'metricId': this.$scope.metric.id, 'rowNumber': this.$scope.rowNumber, 'rowDataListItemId': this.$scope.dataListItem.id });
                }

                this.$scope.inputName = _.camelCase('m' + this.$scope.metric.id) + this.$scope.rowNumber;
            }
            else {
                relatedFormValues = _.filter(this.$scope.ctrl.survey.formValues, { 'metricId': this.$scope.metric.id });
                this.$scope.inputName = _.camelCase('m' + this.$scope.metric.id);
            }

            this.$scope.inputErrorsKey = "surveyForm." + this.$scope.inputName + ".$error";
            this.$scope.viewName = _.camelCase(this.$scope.metric.type);
            this.$scope.isCompact = this.$scope.metricGroup.isRepeater;
            this.$scope.formValues = relatedFormValues;
        }
    }

    angular.module("app").controller("metricController", MetricController);
}