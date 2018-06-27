
module App {
    "use strict";

    interface IOrgConnectionRequestsControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        connectionRequests: Models.IOrgConnectionRequest[];
        displayedConnectionRequests: Models.IOrgConnectionRequest[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        approve: (id: string) => void;
        decline: (id: string) => void;
    }

    interface IOrgConnectionRequestsController {
        activate: () => void;
        approve: (id: string) => void;
        decline: (id: string) => void;
    }

    class OrgConnectionRequestsController implements IOrgConnectionRequestsController {
        static $inject: string[] = ["$scope", "$stateParams", "orgConnectionRequestResource", "toastr"];

        constructor(
            private $scope: IOrgConnectionRequestsControllerScope,
            private $stateParams: ng.ui.IStateParamsService,
            private orgConnectionRequestResource: Resources.IOrgConnectionRequestResource,
            private toastr: any) {

            $scope.title = "Connection Requests";

            $scope.approve = (id) => { this.approve(id); };
            $scope.decline = (id) => { this.decline(id); };

            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            var orgId = this.$stateParams["organisationId"];
            if (orgId !== undefined && orgId.length) {
                this.orgConnectionRequestResource.query({ organisationId: orgId }).$promise.then((connectionRequests) => {
                    this.$scope.connectionRequests = connectionRequests;
                    this.$scope.displayedConnectionRequests = [].concat(this.$scope.connectionRequests);
                });
            } else {
                this.orgConnectionRequestResource.query().$promise.then((connectionRequests) => {
                    this.$scope.connectionRequests = connectionRequests;
                    this.$scope.displayedConnectionRequests = [].concat(this.$scope.connectionRequests);
                });
            }
        }

        approve(id: string) {
            var payload = { id: id };
            this.orgConnectionRequestResource.approve(payload, (res) => {
                this.load();
            }, (err) => {
                console.error(err);
                if (err.data.message)
                    this.toastr.error(err.data.message);
            });
        }

        decline(id: string) {
            this.orgConnectionRequestResource.delete({ id: id },
                () => {
                    this.load();
                },
                (err) => {
                    console.error(err);
                    this.toastr.error(err.data.message);
                });
        }
    }

    angular.module("app").controller("orgConnectionRequestsController", OrgConnectionRequestsController);
}