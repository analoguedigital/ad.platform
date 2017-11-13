
module App {
    "use strict";

    interface IDateMetricFormControllerScope extends ng.IScope {
        metric: Models.IDateMetric;
        close: () => void;
    }

    interface IDateMetricFormController {
        activate: () => void;
    }

    class DateMetricFormController implements IDateMetricFormController {
        static $inject: string[] = ["$scope", "$uibModalInstance", "metric"];

        constructor(private $scope: IDateMetricFormControllerScope,
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

    angular.module("app").controller("dateMetricFormController", DateMetricFormController);
}