
module App {
    "use strict";

    interface IOrganisationInvitationsControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        invitations: Models.IOrgInvitation[];
        displayedInvitations: Models.IOrgInvitation[];
    }

    interface IOrganisationInvitationsController {
        activate: () => void;
    }

    class OrganisationInvitationsController implements IOrganisationInvitationsController {
        static $inject: string[] = ["$scope", "organisationResource", "orgInvitationResource", "toastr", "$stateParams"];

        constructor(
            private $scope: IOrganisationInvitationsControllerScope,
            private organisationResource: Resources.IOrganisationResource,
            private orgInvitationResource: Resources.IOrgInvitationResource,
            private toastr: any,
            private $stateParams: ng.ui.IStateParamsService) {

            $scope.title = "Organisations";
            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            var orgId = this.$stateParams["organisationId"];
            if (orgId !== undefined && orgId.length) {
                this.orgInvitationResource.query({ organisationId: orgId }).$promise.then((invitations) => {
                    this.$scope.invitations = invitations;
                    this.$scope.displayedInvitations = [].concat(this.$scope.invitations);
                });
            } else {
                this.orgInvitationResource.query().$promise.then((invitations) => {
                    this.$scope.invitations = invitations;
                    this.$scope.displayedInvitations = [].concat(this.$scope.invitations);
                });
            }
        }

        delete(id: string) {
            this.orgInvitationResource.delete({ id: id }).$promise.then(() => {
                this.load();
            }, (err) => {
                console.error(err);
            });
        }

    }

    angular.module("app").controller("organisationInvitationsController", OrganisationInvitationsController);
}