
module App {
    "use strict";

    interface IFreeTextMetricFormControllerScope extends ng.IScope {
        metric: Models.IFreeTextMetric;
        close: () => void;
    }

    interface IFreeTextMetricFormController {
        activate: () => void;
    }

    class FreeTextMetricFormController implements IFreeTextMetricFormController {
        static $inject: string[] = ["$scope", "$uibModalInstance", "metric"];

        constructor(private $scope: IFreeTextMetricFormControllerScope,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private metric: Models.IFreeTextMetric) {

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

    angular.module("app").controller("freeTextMetricFormController", FreeTextMetricFormController);
}