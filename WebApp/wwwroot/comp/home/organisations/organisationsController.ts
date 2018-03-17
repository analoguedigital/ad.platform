
module App {
    "use strict";

    interface IOrganisationsControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        organisations: Models.IOrganisation[];
        displayedOrganisations: Models.IOrganisation[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        delete: (id: string) => void;
    }

    interface IOrganisationsController {
        activate: () => void;
        delete: (id: string) => void;
    }

    class OrganisationsController implements IOrganisationsController {
        static $inject: string[] = ["$scope", "organisationResource", "toastr"];

        constructor(
            private $scope: IOrganisationsControllerScope,
            private organisationResource: Resources.IOrganisationResource,
            private toastr: any) {

            $scope.title = "Organisations";
            $scope.delete = (id) => { this.delete(id); };
            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            this.organisationResource.query().$promise.then((organisations) => {
                this.$scope.organisations = organisations;
                this.$scope.displayedOrganisations = [].concat(this.$scope.organisations);
            });
        }

        delete(id: string) {
            this.organisationResource.delete({ id: id },
                () => {
                    this.load();
                },
                (err) => {
                    console.error(err);
                    this.toastr.error(err.data.message);
                });
        }
    }

    angular.module("app").controller("organisationsController", OrganisationsController);
}