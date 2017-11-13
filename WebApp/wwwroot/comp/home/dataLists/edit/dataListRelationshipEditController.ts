
module App {
    "use strict";

    interface IDataListRelationshipEditControllerScope extends ng.IScope {
        relationship: Models.IDataListRelationship;
        dataList: Models.IDataList;
        availableDataLists: Models.IDataListBasic[];
        isAddMode: boolean;
        close: () => void;
        saveAndClose: () => void;
    }

    interface IDataListRelationshipEditController {
        activate: () => void;
    }

    class DataListRelationshipEditController implements IDataListRelationshipEditController {

        static $inject: string[] = ["$scope", "$uibModalInstance", "dataListRelationship", "dataList", "dataListResource"];
        constructor(private $scope: IDataListRelationshipEditControllerScope,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private dataListRelationship: Models.IDataListRelationship,
            private dataList: Models.IDataList,
            private dataListsResource: Resources.IDataListResource) {

            this.$scope.isAddMode = false;
            if (dataListRelationship === null) {
                this.$scope.isAddMode = true;
                $scope.relationship = <Models.IDataListRelationship>{};
            }
            else {
                $scope.relationship = dataListRelationship;
            }
            $scope.dataList = dataList;
            dataListsResource.list().$promise.then((datalists) => {
                this.$scope.availableDataLists = <Models.IDataListBasic[]>_.differenceBy(datalists, _.concat(_.map(this.dataList.relationships, 'dataList'), this.dataList), (a: any) => { return a.id; });
            });

            $scope.close = () => { this.close(); };
            $scope.saveAndClose = () => { this.saveAndClose(); };
            this.activate();
        }

        activate() {

        }

        saveAndClose() {
            this.$scope.relationship.dataListId = this.$scope.relationship.dataList.id;
            this.$uibModalInstance.close(this.$scope.relationship);
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("dataListRelationshipEditController", DataListRelationshipEditController);
}