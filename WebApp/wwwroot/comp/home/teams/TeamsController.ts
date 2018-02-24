
module App {
    "use strict";

    interface ITeamsControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        teams: Models.IOrgTeam[];
        displayedTeams: Models.IOrgTeam[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
    }

    interface ITeamsController {
        activate: () => void;
        delete: (id: string) => void;
    }

    class TeamsController implements ITeamsController {
        static $inject: string[] = ["$scope", "$stateParams", "$uibModal", "toastr", "orgTeamResource", "orgUserResource"];

        constructor(
            private $scope: ITeamsControllerScope,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private toastr: any,
            private orgTeamResource: Resources.IOrgTeamResource,
            private userResource: Resources.IOrgUserResource) {

            $scope.title = "Organisation Teams";
            $scope.delete = (id: string) => { this.delete(id); };

            this.activate();
        }

        load() {
            var organisationId = this.$stateParams["organisationId"];
            if (organisationId === '') {
                this.orgTeamResource.query().$promise.then((teams) => {
                    this.$scope.teams = teams;
                    this.$scope.displayedTeams = [].concat(this.$scope.teams);
                });
            }
            else {
                this.orgTeamResource.query({ organisationId: organisationId }).$promise.then((teams) => {
                    this.$scope.teams = teams;
                    this.$scope.displayedTeams = [].concat(this.$scope.teams);
                });
            }
        }

        activate() {
            this.load();
        }

        delete(id: string) {
            console.warn('delete team', id);

            this.orgTeamResource.delete({ id: id },
                () => { this.load(); },
                (err) => { console.log(err); });
        }
    }

    angular.module("app").controller("teamsController", TeamsController);
}