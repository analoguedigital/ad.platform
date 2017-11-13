
module App {
    "use strict";

    interface ITimeMetricController {
        activate: () => void;
    }

    interface ITimeMetricContrllerScope extends IMetricControllerScope {
        metric: Models.ITimeMetric;
        metricGroup: Models.IMetricGroup;
    }


    class TimeMetricController implements ITimeMetricController {

        static $inject: string[] = ['$scope'];

        constructor(private $scope: ITimeMetricContrllerScope) {
            this.activate();
        }


        activate() {
            if (_.isEmpty(this.$scope.formValues)) {
                this.$scope.formValue = this.$scope.ctrl.addFormValue(this.$scope.metric, this.$scope.dataListItem, this.$scope.rowNumber);
                this.$scope.formValue.timeValue = new Date();
            }
            else {
                this.$scope.formValue = this.$scope.formValues[0];
            }
        }
    }

    angular.module("app").controller("timeMetricController", TimeMetricController);
}