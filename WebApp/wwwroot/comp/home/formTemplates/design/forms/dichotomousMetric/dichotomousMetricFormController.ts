
module App {
    "use strict";

    interface IDichotomousMetricFormControllerScope extends ng.IScope {
        metric: Models.IDichotomousMetric;
        close: () => void;
    }

    interface IDichotomousMetricFormController {
        activate: () => void;
    }

    class DichotomousMetricFormController implements IDichotomousMetricFormController {
        static $inject: string[] = ["$scope", "$uibModalInstance", "metric"];

        constructor(private $scope: IDichotomousMetricFormControllerScope,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private metric: Models.IDichotomousMetric) {

            $scope.metric = metric;
            $scope.close = () => { this.close(); };
            this.activate();
        }

        activate() {

        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("dichotomousMetricFormController", DichotomousMetricFormController);
}