
module App {
    "use strict";

    interface IOrganisationInvitationsEditControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        currentUserIsSuperUser: boolean;

        invitation: any;
        organisations: Models.IOrganisation[];
    }

    interface IOrganisationInvitationsEditController {
        activate: () => void;
    }

    class OrganisationInvitationsEditController implements IOrganisationInvitationsEditController {
        static $inject: string[] = ["$scope", "$state", "$stateParams", "orgInvitationResource", "organisationResource", "toastr", "userContextService"];
        constructor(
            private $scope: IOrganisationInvitationsEditControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private orgInvitationResource: Resources.IOrgInvitationResource,
            private organisationResource: Resources.IOrganisationResource,
            private toastr: any,
            private userContextService: Services.IUserContextService) {

            $scope.title = "Organization Invitation";
            this.activate();
        }

        activate() {
            var roles = ["System administrator", "Platform administrator"];
            this.$scope.currentUserIsSuperUser = this.userContextService.userIsInAnyRoles(roles);

            this.load();
        }

        load() {
            var invitationId = this.$stateParams["id"];
            if (invitationId == '') {
                this.$scope.invitation = <Models.IOrgInvitation>{
                    id: '',
                    name: '',
                    token: '',
                    limit: 1,
                    used: 0,
                    isActive: true,
                    organisation: {}
                };
            } else {
                this.orgInvitationResource.get({ id: invitationId }).$promise.then((invitation) => {
                    this.$scope.invitation = invitation;
                });
            }

            this.organisationResource.query().$promise.then((organisations) => {
                this.$scope.organisations = organisations;
            });
        }

        generateToken() {
            var generator = function guid() {
                function s4() {
                    return Math.floor((1 + Math.random()) * 0x10000)
                        .toString(16)
                        .substring(1);
                }
                return s4() + '-' + s4();
            };

            var token = generator();
            this.$scope.invitation.token = token;
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }
            var invitationId = this.$stateParams['id'];
            if (invitationId === '') {
                this.orgInvitationResource.save(
                    this.$scope.invitation,
                    () => { this.$state.go('home.organisations.invitations'); },
                    (err) => {
                        console.log(err);
                        this.$scope.errors = err.data.exceptionMessage;

                        this.toastr.error(err.data.message);
                    });
            }
            else {
                this.orgInvitationResource.update(
                    this.$scope.invitation,
                    () => { this.$state.go('home.organisations.invitations'); },
                    (err) => {
                        console.log(err);
                        this.$scope.errors = err.data.exceptionMessage;
                    });
            }
        }

    }

    angular.module("app").controller("organisationInvitationsEditController", OrganisationInvitationsEditController);
}