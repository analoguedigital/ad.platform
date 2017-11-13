
module App {
    "use strict";

    interface IMetricGroupFormControllerScope extends ng.IScope {
        group: Models.IMetricGroup;
        globalDataLists: Models.IDataList[];

        close: () => void;
        addAdhocItem: () => void;
        changeIsAdHoc: () => void;
        removeAdhocItem: (adhocItem: Models.IDataListItem) => void;
    }

    interface IMetricGroupFormController {
        activate: () => void;
    }

    class MetricGroupFormController implements IMetricGroupFormController {
        static $inject: string[] = ["$scope", "$uibModalInstance", "group", "dataListResource"];

        constructor(private $scope: IMetricGroupFormControllerScope,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private group: Models.IMetricGroup,
            private dataListResource: Resources.IDataListResource) {

            $scope.group = group;
            $scope.close = () => { this.close(); };
            $scope.addAdhocItem = () => { this.addAdhocItem(); };
            $scope.removeAdhocItem = (adhocItem) => { this.removeAdhocItem(adhocItem); }
            $scope.changeIsAdHoc = () => { this.changeIsAdHoc(); }

            this.activate();
        }

        activate() {
            this.dataListResource.get().$promise.then((data: any) => {
                this.$scope.globalDataLists = data.items;
            });
        }

        removeAdhocItem(adhocItem: Models.IDataListItem) {
            adhocItem.isDeleted = true;
        }


        addAdhocItem() {
            // create a new items with a temproray id 
            this.$scope.group.adHocItems.push(<Models.IDataListItem>{ isDeleted: false, id: Date.now().toString() });
        }

        changeIsAdHoc() {
            this.$scope.group.dataListId = null;
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
            this.$scope.group = Object.create(this.$scope.group);
        };
    }

    angular.module("app").controller("metricGroupFormController", MetricGroupFormController);
}