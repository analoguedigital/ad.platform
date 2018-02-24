
module App {
    "use strict";

    interface IOrganisationTeamEditControllerScope extends ng.IScope {
        title: string;
        team: Models.IOrgTeam;
        submit: (form: ng.IFormController) => void;
        remove: (user: Models.IOrgTeamUser) => void;
        clearErrors: () => void;
    }

    interface IOrganisationTeamEditController {
        activate: () => void;
    }

    class OrganisationTeamEditController implements IOrganisationTeamEditController {
        static $inject: string[] = ["$scope", "$state", "$stateParams", "orgTeamResource", "toastr"];

        constructor(
            private $scope: IOrganisationTeamEditControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private orgTeamResource: Resources.IOrgTeamResource,
            private toastr: any
        ) {

            $scope.title = "Organisation Teams";

            $scope.minicolorSettings = {
                control: 'hue',
                format: 'hex',
                opacity: false,
                theme: 'bootstrap',
                position: 'top left'
            };

            $scope.updateStatus = (id, flag) => { this.updateStatus(id, flag); };
            $scope.remove = (user: Models.IOrgTeamUser) => { this.remove(user); };
            $scope.submit = (form: ng.IFormController) => { this.submit(form); }
            $scope.clearErrors = () => { this.clearErrors(); }

            this.activate();
        }

        activate() {
            var teamId = this.$stateParams['id'];
            if (teamId === '')
                teamId = '00000000-0000-0000-0000-000000000000';

            this.orgTeamResource.get({ id: teamId }).$promise.then((team) => {
                this.$scope.team = team;
            });
        }

        updateStatus(userId: string, flag: boolean) {
            var payload = {
                id: this.$scope.team.id,
                userId: userId,
                flag: flag
            };

            this.orgTeamResource.updateStatus(payload, (res) => { });
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            var teamId = this.$stateParams['id'];
            if (teamId === '') {
                this.orgTeamResource.save(
                    this.$scope.team,
                    () => { this.$state.go('home.teams.list'); },
                    (err) => {
                        console.log(err);
                        this.$scope.errors = err.data.exceptionMessage;
                    });
            }
            else {
                this.orgTeamResource.update(
                    this.$scope.team,
                    () => { this.$state.go('home.teams.list'); },
                    (err) => {
                        console.log(err);
                        this.$scope.errors = err.data.exceptionMessage;
                    });
            }
        }

        remove(member: Models.IOrgTeamUser) {
            this.orgTeamResource.removeUser({ id: this.$scope.team.id, userId: member.id }, (res) => {
                var item = _.filter(this.$scope.team.users, (user) => { return user.id == member.id; })[0];
                var index = this.$scope.team.users.indexOf(item);
                this.$scope.team.users.splice(index, 1);

                this.toastr.success('User removed from team');
            });
        }

        clearErrors() {
            this.$scope.errors = undefined;
        }

    }

    angular.module("app").controller("organisationTeamEditController", OrganisationTeamEditController);
}