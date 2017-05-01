
module App {
    "use strict";

    interface IDichotomousMetricController {
        activate: () => void;
    }

    interface IDichotomousMetricControllerScope extends IMetricControllerScope {
        metric: Models.IDichotomousMetric;
        metricGroup: Models.IMetricGroup;
    }

    class DichotomousMetricController implements IDichotomousMetricController {

        static $inject: string[] = ['$scope'];

        constructor(private $scope: IDichotomousMetricControllerScope) {
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

    angular.module("app").controller("dichotomousMetricController", DichotomousMetricController);
}