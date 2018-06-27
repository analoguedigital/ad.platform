
module App {
    "use strict";

    interface IOrgRequestsControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        requests: Models.IOrgRequest[];
        displayedRequests: Models.IOrgRequest[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        delete: (id: string) => void;
    }

    interface IOrgRequestsController {
        activate: () => void;
        delete: (id: string) => void;
    }

    class OrgRequestsController implements IOrgRequestsController {
        static $inject: string[] = ["$scope", "$stateParams", "orgRequestResource", "toastr"];

        constructor(
            private $scope: IOrgRequestsControllerScope,
            private $stateParams: ng.ui.IStateParamsService,
            private orgRequestResource: Resources.IOrgRequestResource,
            private toastr: any) {

            $scope.title = "Organisation Requests";
            $scope.delete = (id) => { this.delete(id); };

            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            this.orgRequestResource.query().$promise.then((requests) => {
                this.$scope.requests = requests;
                this.$scope.displayedRequests = [].concat(this.$scope.requests);
            });
        }

        delete(id: string) {
            this.orgRequestResource.delete({ id: id },
                () => {
                    this.load();
                },
                (err) => {
                    console.error(err);
                    this.toastr.error(err.data.message);
                });
        }
    }

    angular.module("app").controller("orgRequestsController", OrgRequestsController);
}