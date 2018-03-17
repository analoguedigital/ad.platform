
module App {
    "use strict";

    interface IProjectEditControllerScope extends ng.IScope {
        title: string;
        openStartDateCalendar: () => void;
        openEndDateCalendar: () => void;
        startDateCalendar: any;
        endDateCalendar: any;
        project: Models.IProject;
        assignments: Models.IProjectAssignment[];
        teams: Models.IOrgTeam[];
        errors: string;
        submit: (form: ng.IFormController) => void;
        clearErrors: () => void;
        currentUserIsSuperUser: boolean;
        organisations: Models.IOrganisation[];
    }

    interface IProjectEditController {
        activate: () => void;
    }

    class ProjectEditController implements IProjectEditController {
        private isInsertMode: boolean = false;

        static $inject: string[] = ["$scope", "projectResource", "$state", "$stateParams", "organisationResource", "userContextService"];
        constructor(
            private $scope: IProjectEditControllerScope,
            private projectResource: Resources.IProjectResource,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private organisationResource: Resources.IOrganisationResource,
            private userContextService: Services.IUserContextService
        ) {

            $scope.title = "Cases";
            $scope.startDateCalendar = {
                isOpen: false
            };
            $scope.endDateCalendar = {
                isOpen: false
            };

            $scope.openStartDateCalendar = () => { this.openStartDateCalendar(); }
            $scope.openEndDateCalendar = () => { this.openEndDateCalendar(); }
            $scope.submit = (form: ng.IFormController) => { this.submit(form); }
            $scope.clearErrors = () => { this.clearErrors(); }

            this.activate();
        }

        activate() {
            var projectId = this.$stateParams['id'];
            if (projectId === '')
            {
                projectId = '00000000-0000-0000-0000-000000000000';
                this.isInsertMode = true;
                this.projectResource.get({ id: projectId }).$promise.then((project) => {
                    this.$scope.project = project;
                });
            } else {
                this.projectResource.get({ id: projectId }).$promise.then((project) => {
                    this.$scope.project = project;

                    this.projectResource.assignments({ id: projectId }, (assignments) => {
                        this.$scope.assignments = assignments;
                    });

                    this.projectResource.teams({ id: projectId }, (teams) => {
                        this.$scope.teams = teams;
                    });
                });
            }

            var roles = ["System administrator", "Platform administrator"];
            this.$scope.currentUserIsSuperUser = this.userContextService.userIsInAnyRoles(roles);

            if (this.$scope.currentUserIsSuperUser) {
                this.organisationResource.query().$promise.then((organisations) => {
                    this.$scope.organisations = organisations;
                });
            }
        }

        openStartDateCalendar() {
            this.$scope.startDateCalendar.isOpen = true;
        }

        openEndDateCalendar() {
            this.$scope.endDateCalendar.isOpen = true;
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }
            var projectId = this.$stateParams['id'];
            if (projectId === '') {
                this.projectResource.save(
                    this.$scope.project,
                    () => { this.$state.go('home.projects.list'); },
                    (err) => {
                        console.log(err);
                        this.$scope.errors = err.data.exceptionMessage;
                    });
            }
            else {
                this.projectResource.update(
                    this.$scope.project,
                    () => { this.$state.go('home.projects.list'); },
                    (err) => {
                        console.log(err);
                        this.$scope.errors = err.data.exceptionMessage;
                    });
            }
        }

        clearErrors() {
            this.$scope.errors = undefined;
        }

    }

    angular.module("app").controller("projectEditController", ProjectEditController);
}