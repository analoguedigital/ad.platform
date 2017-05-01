
module App {
    "use strict";

    interface IMultipleChoiceMetricFormControllerScope extends ng.IScope {
        metric: Models.IMultipleChoiceMetric;
        globalDataLists: Models.IDataListBasic[];

        close: () => void;
        addAdhocItem: () => void;
        changeIsAdHoc: () => void;
        removeAdhocItem: (adhocItem: Models.IDataListItem) => void;
    }

    interface IMultipleChoiceMetricFormController {
        activate: () => void;
    }

    class MultipleChoiceMetricFormController implements IMultipleChoiceMetricFormController {
        static $inject: string[] = ["$scope", "$uibModalInstance", "metric", "dataListResource"];

        constructor(private $scope: IMultipleChoiceMetricFormControllerScope,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private metric: Models.IMultipleChoiceMetric,
            private dataListResource: Resources.IDataListResource) {

            $scope.metric = metric;
            dataListResource.list().$promise.then((datalists) => {
                $scope.globalDataLists = datalists;
            });
            
            $scope.close = () => { this.close(); };
            $scope.addAdhocItem = () => { this.addAdhocItem(); };
            $scope.removeAdhocItem = (adhocItem) => { this.removeAdhocItem(adhocItem); }
            $scope.changeIsAdHoc = () => { this.changeIsAdHoc(); }
            this.activate();
        }

        activate() {

        }

        removeAdhocItem(adhocItem: Models.IDataListItem) {
            adhocItem.isDeleted = true;
        }


        addAdhocItem() {
            // create a new items with a temproray id 
            this.$scope.metric.adHocItems.push(<Models.IDataListItem>{ isDeleted: false, id: Date.now().toString() }); 
        }

        changeIsAdHoc() {
            this.$scope.metric.dataListId = null;
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
            this.$scope.metric = Object.create(this.$scope.metric);
        };
    }

    angular.module("app").controller("multipleChoiceMetricFormController", MultipleChoiceMetricFormController);
}