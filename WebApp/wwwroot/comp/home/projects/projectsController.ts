module App {
    "use strict";

    interface IAddAdviceThreadModel {
        title: string;
        description: string;
        colour: string;
    }

    interface IProjectsControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        safeProjects: Models.IProject[];
        projects: Models.IProject[];
        displayedProjects: Models.IProject[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
        minicolorSettings: any;
        model: IAddAdviceThreadModel;
        accountTypeFilter: string;

        delete: (id: string) => void;
    }

    interface IProjectsController {
        activate: () => void;
        delete: (id: string) => void;
    }

    class ProjectsController implements IProjectsController {
        static $inject: string[] = ["$scope", "$state", "$stateParams", "$uibModal", "projectResource", "userContextService", "localStorageService"];

        constructor(
            private $scope: IProjectsControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private projectResource: Resources.IProjectResource,
            private userContextService: Services.IUserContextService,
            private localStorageService: ng.local.storage.ILocalStorageService) {

            $scope.title = "Staff and Clients";

            $scope.delete = (id) => { this.delete(id); };

            var orgUser = this.userContextService.current.orgUser;
            if (orgUser !== null && orgUser.accountType === 0) {
                this.$state.go("home.dashboard.layout");
            }

            this.activate();
        }

        activate() {
            this.$scope.minicolorSettings = {
                control: 'hue',
                format: 'hex',
                opacity: false,
                theme: 'bootstrap',
                position: 'top left'
            };

            this.load();
        }

        load() {
            var accountTypeFilter = <string>this.localStorageService.get('projects_account_type_filter');
            if (accountTypeFilter == null || !accountTypeFilter.length) {
                accountTypeFilter = 'all';
            }

            var organisationId = this.$stateParams["organisationId"];
            if (organisationId === '') {
                this.projectResource.query().$promise.then((projects) => {
                    this.$scope.safeProjects = projects;
                    this.$scope.projects = projects;
                    this.$scope.displayedProjects = [].concat(this.$scope.projects);

                    this.filterProjects(accountTypeFilter);
                });
            } else {
                this.projectResource.query({ organisationId: organisationId }).$promise.then((projects) => {
                    this.$scope.safeProjects = projects;
                    this.$scope.projects = projects;
                    this.$scope.displayedProjects = [].concat(this.$scope.projects);

                    this.filterProjects(accountTypeFilter);
                });
            }
        }

        addAdviceThread(id: string) {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/projects/addAdviceThread/addAdviceThreadModalView.html',
                controller: 'addAdviceThreadModalController',
                controllerAs: 'ctrl',
                resolve: {
                    projectId: () => {
                        return id;
                    }
                }
            }).result.then(
                (res) => {
                    // advice thread created
                },
                (err) => { });
        }

        addNewProject() {
            this.$state.go('home.projects.edit');
        }

        delete(id: string) {
            this.projectResource.delete({ id: id },
                () => { this.load(); },
                (err) => { console.log(err); });
        }

        filterProjects(type: string) {
            this.$scope.accountTypeFilter = type;
            this.localStorageService.set('projects_account_type_filter', type);

            if (type === 'clients') {
                this.$scope.projects = _.filter(this.$scope.safeProjects, (p) => { return p.createdBy !== null && p.createdBy.accountType === 0; });
            } else if (type === 'staff') {
                this.$scope.projects = _.filter(this.$scope.safeProjects, (p) => { return p.createdBy === null || p.createdBy.accountType === 1; });
            } else if (type === 'all') {
                this.$scope.projects = this.$scope.safeProjects;
            }

            this.$scope.displayedProjects = [].concat(this.$scope.projects);
        }

    }

    angular.module("app").controller("projectsController", ProjectsController);
}