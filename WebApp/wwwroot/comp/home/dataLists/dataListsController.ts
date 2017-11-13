
module App {
    "use strict";

    interface IDataListsControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        dataLists: Models.IDataListBasic[];
        displayedDataLists: Models.IDataListBasic[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        delete: (id: string) => void;
    }

    interface IDataListsController {
        activate: () => void;
        delete: (id: string) => void;
    }

    class DataListsController implements IDataListsController {
        static $inject: string[] = ["$scope", "dataListResource", "toastr"];

        constructor(
            private $scope: IDataListsControllerScope,
            private dataListResource: Resources.IDataListResource,
            private toastr: any) {

            $scope.title = "Data Lists";
            $scope.delete = (id) => { this.delete(id); };
            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            this.dataListResource.list().$promise.then((dataLists) => {
                this.$scope.dataLists = _.filter(dataLists, (dataList) => { return dataList.name !== null });
                this.$scope.displayedDataLists = [].concat(this.$scope.dataLists);
            });

        }

        delete(id: string) {
            this.dataListResource.delete({ id: id },
                () => { this.load(); },
                (err) => {
                    this.toastr.error(err.data.message);
                    console.log(err);
                });
        }
    }

    angular.module("app").controller("dataListsController", DataListsController);
}