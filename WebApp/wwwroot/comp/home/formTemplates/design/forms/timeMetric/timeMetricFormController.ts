
module App {
    "use strict";

    interface ITimeMetricFormControllerScope extends ng.IScope {
        metric: Models.IDateMetric;
        close: () => void;
    }

    interface ITimeMetricFormController {
        activate: () => void;
    }

    class TimeMetricFormController implements ITimeMetricFormController {
        static $inject: string[] = ["$scope", "$uibModalInstance", "metric"];

        constructor(private $scope: ITimeMetricFormControllerScope,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private metric: Models.IDateMetric) {

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

    angular.module("app").controller("timeMetricFormController", TimeMetricFormController);
}