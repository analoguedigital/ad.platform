
module App {
    "use strict";

    interface IRateMetricFormControllerScope extends ng.IScope {
        metric: Models.IRateMetric;
        close: () => void;
    }

    interface IRateMetricFormController {
        activate: () => void;
    }

    class RateMetricFormController implements IRateMetricFormController {
        static $inject: string[] = ["$scope", "$uibModalInstance", "metric"];

        constructor(private $scope: IRateMetricFormControllerScope,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private metric: Models.IRateMetric) {

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

    angular.module("app").controller("rateMetricFormController", RateMetricFormController);
}