
module App {
    "use strict";

    interface INumericMetricController {
        activate: () => void;
    }

    interface INumericMetricControllerScope extends IMetricControllerScope
    {
        metric: Models.INumericMetric;
        metricGroup: Models.IMetricGroup;
    }

    class NumericMetricController implements INumericMetricController {

        static $inject: string[] = ['$scope'];

        constructor(private $scope: INumericMetricControllerScope) {

            this.activate();
        }

        activate() {
            if (_.isEmpty(this.$scope.formValues)) {
                this.$scope.formValue = this.$scope.ctrl.addFormValue(this.$scope.metric, this.$scope.dataListItem, this.$scope.rowNumber);
            }
            else {
                this.$scope.formValue = this.$scope.formValues[0];
            }
        }
    }

    angular.module("app").controller("numericMetricController", NumericMetricController);
}