module App {
    "use strict";

    interface IMetricController {
        survey: Models.ISurvey;
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
        isViewMode: boolean;
        isPrintMode: boolean;
        isExportMode: boolean;
    }

    class MetricController implements IMetricController {


        static readonly surveyViewRouteName: string = "home.surveys.view";
        static readonly surveyPrintRouteNames: string[] = ["home.surveys.print-single", "home.surveys.print-multiple"];
        static readonly projectSummaryPrintRouteName: string = "home.projects.summaryPrint";

        survey: Models.ISurvey;

        static $inject: string[] = ["$scope", "$state"];
        constructor(
            private $scope: IMetricControllerScope,
            private $state: angular.ui.IStateService) {

            this.activate();
            $scope.$watch('metric', () => { this.activate(); });
        }

        activate() {

            if (this.$state.current.name == MetricController.surveyViewRouteName) {
                this.$scope.isViewMode = true;
            }

            if (MetricController.surveyPrintRouteNames.indexOf(this.$state.current.name) !== -1) {
                this.$scope.isPrintMode = true;
            }

            if (this.$state.current.name == MetricController.projectSummaryPrintRouteName) {
                this.$scope.isExportMode = true;
            }

            var relatedFormValues: Models.IFormValue[] = [];

            if (this.$scope.ctrl.survey) {
                this.survey = this.$scope.ctrl.survey;
            } else {
                this.survey = this.$scope.survey;
            }


            if (this.$scope.metricGroup.isRepeater) {
                try {
                    if (this.$scope.metricGroup.type === "IterativeRepeater") {
                        relatedFormValues = _.filter(this.survey.formValues, { 'metricId': this.$scope.metric.id, 'rowNumber': this.$scope.rowNumber });
                    } else {
                        relatedFormValues = _.filter(this.survey.formValues, { 'metricId': this.$scope.metric.id, 'rowNumber': this.$scope.rowNumber, 'rowDataListItemId': this.$scope.dataListItem.id });
                    }
                } catch (e) {
                    console.error('relatedFormValues error', e);
                }

                this.$scope.inputName = _.camelCase('m' + this.$scope.metric.id) + this.$scope.rowNumber;
            }
            else {
                relatedFormValues = _.filter(this.survey.formValues, { 'metricId': this.$scope.metric.id });
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