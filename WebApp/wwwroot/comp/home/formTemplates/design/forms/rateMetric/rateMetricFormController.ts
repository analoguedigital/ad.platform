
module App {
    "use strict";

    interface IRateMetricFormControllerScope extends ng.IScope {
        metric: Models.IRateMetric;
        close: () => void;
        addAdhocItem: () => void;
        changeIsAdHoc: () => void;
        removeAdhocItem: (adhocItem: Models.IDataListItem) => void;
    }

    interface IRateMetricFormController {
        activate: () => void;
    }

    class RateMetricFormController implements IRateMetricFormController {
        static $inject: string[] = ["$scope", "$uibModalInstance", "toastr", "metric"];

        constructor(private $scope: IRateMetricFormControllerScope,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private toastr: any,
            private metric: Models.IRateMetric) {

            $scope.metric = metric;

            $scope.close = () => { this.close(); };
            $scope.addAdhocItem = () => { this.addAdhocItem(); };
            $scope.removeAdhocItem = (adhocItem) => { this.removeAdhocItem(adhocItem); }
            $scope.changeIsAdHoc = () => { this.changeIsAdHoc(); }

            this.activate();
        }

        activate() {
            this.$scope.metric.isAdHoc = this.$scope.metric.adHocItems.length > 0;
        }

        removeAdhocItem(adhocItem: Models.IDataListItem) {
            adhocItem.isDeleted = true;
        }


        addAdhocItem() {
            // create a new items with a temporary id 
            this.$scope.metric.adHocItems.push(<Models.IDataListItem>{ isDeleted: false, id: Date.now().toString() });
        }

        changeIsAdHoc() {
            this.$scope.metric.dataListId = null;
        }

        close() {
            if (this.$scope.metric.isAdHoc && this.$scope.metric.adHocItems.length > 0) {
                var adHocItems = this.$scope.metric.adHocItems
                    .map((item) => { return item.value });
                var hasDuplicates = _.uniq(adHocItems).length !== adHocItems.length;
                if (hasDuplicates) {
                    this.toastr.error('Ad-hoc list cannot contain duplicate values!', 'Error', { closeButton: true });
                    return false;
                }
            }

            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("rateMetricFormController", RateMetricFormController);
}