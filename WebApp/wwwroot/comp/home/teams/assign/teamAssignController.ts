module App {
    "use strict";

    interface IOrganisationTeamAssignControllerScope extends ng.IScope {
        title: string;
        team: Models.IOrgTeam;
        users: Models.IOrgUser[];
        assignments: ITeamUserAssignment[];
        displayedAssignments: ITeamUserAssignment[];
        searchTerm: string;
    }

    interface ITeamUserAssignment {
        userId: string;
        userName: string;
        isRootUser: boolean;
        isWebUser: boolean;
        isMobileUser: boolean;
        isMember: boolean;
        isManager: boolean;
    }

    interface IOrganisationTeamAssignController {
        activate: () => void;
    }

    class OrganisationTeamAssignController implements IOrganisationTeamAssignController {
        static $inject: string[] = ["$scope", "$state", "$stateParams", "orgTeamResource", "orgUserResource", "toastr"];

        constructor(
            private $scope: IOrganisationTeamAssignControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private orgTeamResource: Resources.IOrgTeamResource,
            private orgUserResource: Resources.IOrgUserResource,
            private toastr: any
        ) {
            $scope.title = "Organisation Teams";
            this.activate();
        }

        activate() {
            var teamId = this.$stateParams['id'];
            this.orgTeamResource.get({ id: teamId }).$promise.then((team) => {
                this.$scope.team = team;

                this.orgTeamResource.getAssignableUsers({ id: team.id }, (users) => {
                    this.$scope.users = users;
                    this.$scope.assignments = [];

                    _.forEach(users, (user: Models.IOrgUser) => {
                        this.$scope.assignments.push({
                            userId: user.id,
                            userName: `${user.firstName} ${user.surname}`,
                            isRootUser: user.isRootUser,
                            isWebUser: user.isWebUser,
                            isMobileUser: user.isMobileUser,
                            isMember: false,
                            isManager: false
                        });
                    });

                    this.$scope.displayedAssignments = [].concat(this.$scope.assignments);
                });
            });
        }

        submit() {
            var res = [];
            var selectedUsers = _.filter(this.$scope.assignments, (assg) => { return assg.isMember; });
            _.forEach(selectedUsers, (assignment) => {
                res.push({
                    orgUserId: assignment.userId,
                    isManager: assignment.isManager
                });
            });

            var payload = {
                id: this.$scope.team.id,
                users: res
            };

            this.orgTeamResource.assign(payload, (result) => {
                this.toastr.success('Selected users assigned to team');
            });
        }

    }

    angular.module("app").controller("organisationTeamAssignController", OrganisationTeamAssignController);
}