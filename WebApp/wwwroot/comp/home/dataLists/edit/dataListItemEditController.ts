
module App {
    "use strict";

    interface IDataListItemEditControllerScope extends ng.IScope {
        dataListItem: Models.IDataListItem;
        dataList: Models.IDataList;
        attributes: any;
        isAddMode: boolean;
        close: () => void;
        saveAndClose: () => void;
        getAttribute: (item: Models.IDataListItem, relationship: Models.IDataListRelationship) => Models.IDataListItemAttr;
    }

    interface IDataListItemEditController {
        activate: () => void;
    }

    class DataListItemEditController implements IDataListItemEditController {

        static $inject: string[] = ["$scope", "$uibModalInstance", "dataListItem", "dataList", "dataListResource"];
        constructor(private $scope: IDataListItemEditControllerScope,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private dataListItem: Models.IDataListItem,
            private dataList: Models.IDataList,
            private dataListResource: Resources.IDataListResource) {

            this.$scope.isAddMode = false;
            if (dataListItem === null) {
                this.$scope.isAddMode = true;
                $scope.dataListItem = <Models.IDataListItem>{ isDeleted: false };
            }
            else {
                $scope.dataListItem = dataListItem;
            }
            $scope.dataList = dataList;
            $scope.close = () => { this.close(); };
            $scope.saveAndClose = () => { this.saveAndClose(); };
            $scope.getAttribute = (item, relationship) => { return this.getAttribute(item, relationship); }

            $scope.attributes = {};

            angular.forEach($scope.dataList.relationships, (relationship) => { $scope.attributes[relationship.id] = this.getAttribute($scope.dataListItem, relationship); });
            this.activate();
        }

        getAttribute(item: Models.IDataListItem, relationship: Models.IDataListRelationship) {
            return _.filter(item.attributes, { relationshipId: relationship.id })[0];
        }

        activate() {

        }

        saveAndClose() {
            this.$uibModalInstance.close(this.$scope.dataListItem);
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("dataListItemEditController", DataListItemEditController);
}