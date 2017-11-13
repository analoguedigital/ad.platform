
module App {
    "use strict";

    interface INumericMetricFormControllerScope extends ng.IScope {
        metric: Models.INumericMetric;
        close: () => void;
    }

    interface INumericMetricFormController {
        activate: () => void;
    }

    class NumericMetricFormController implements INumericMetricFormController {
        static $inject: string[] = ["$scope", "$uibModalInstance", "metric"];

        constructor(private $scope: INumericMetricFormControllerScope,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private metric: Models.INumericMetric) {

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

    angular.module("app").controller("numericMetricFormController", NumericMetricFormController);
}