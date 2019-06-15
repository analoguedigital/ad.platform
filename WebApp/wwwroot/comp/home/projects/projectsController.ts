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

        clientsCount: number;
        staffCount: number;
        groupsCount: number;
        totalCount: number;

        delete: (id: string) => void;
    }

    interface IProjectsController {
        activate: () => void;
        delete: (id: string) => void;
    }

    class ProjectsController implements IProjectsController {
        static $inject: string[] = ["$scope", "$state", "$stateParams", "$uibModal", "projectResource", "userContextService", "localStorageService", "toastr"];

        constructor(
            private $scope: IProjectsControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private projectResource: Resources.IProjectResource,
            private userContextService: Services.IUserContextService,
            private localStorageService: ng.local.storage.ILocalStorageService,
            private toastr: any) {

            $scope.title = "Create threads, groups and records";

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
                accountTypeFilter = 'clients';
            }

            var organisationId = this.$stateParams["organisationId"];
            if (organisationId === '') {
                this.projectResource.query().$promise.then((projects) => {
                    this.$scope.safeProjects = projects;
                    this.$scope.projects = projects;
                    this.$scope.displayedProjects = [].concat(this.$scope.projects);

                    this.countProjectTypes();
                    this.filterProjects(accountTypeFilter);
                });
            } else {
                this.projectResource.query({ organisationId: organisationId }).$promise.then((projects) => {
                    this.$scope.safeProjects = projects;
                    this.$scope.projects = projects;
                    this.$scope.displayedProjects = [].concat(this.$scope.projects);

                    this.countProjectTypes();
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

        addRecordThread(id: string) {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/projects/addRecordThread/addRecordThreadModalView.html',
                controller: 'addRecordThreadModalController',
                controllerAs: 'ctrl',
                resolve: {
                    projectId: () => {
                        return id;
                    }
                }
            }).result.then(
                (res) => {
                    // record thread created
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

        countProjectTypes() {
            this.$scope.clientsCount = _.filter(this.$scope.safeProjects, (p) => { return p.createdBy !== null && p.createdBy.accountType === 0; }).length;
            this.$scope.staffCount = _.filter(this.$scope.safeProjects, (p) => { return (p.createdBy === null || p.createdBy.accountType === 1) && !p.isAggregate; }).length;
            this.$scope.groupsCount = _.filter(this.$scope.safeProjects, (p) => { return p.isAggregate === true; }).length;
            this.$scope.totalCount = this.$scope.clientsCount + this.$scope.staffCount + this.$scope.groupsCount;
        }

        filterProjects(type: string) {
            this.$scope.accountTypeFilter = type;
            this.localStorageService.set('projects_account_type_filter', type);

            if (type === 'clients') {
                this.$scope.projects = _.filter(this.$scope.safeProjects, (p) => { return p.createdBy !== null && p.createdBy.accountType === 0; });
            } else if (type === 'staff') {
                this.$scope.projects = _.filter(this.$scope.safeProjects, (p) => { return (p.createdBy === null || p.createdBy.accountType === 1) && !p.isAggregate; });
            } else if (type === 'groups') {
                this.$scope.projects = _.filter(this.$scope.safeProjects, (p) => { return p.isAggregate === true; });
            }
            else if (type === 'all') {
                this.$scope.projects = this.$scope.safeProjects;
            }

            this.$scope.displayedProjects = [].concat(this.$scope.projects);
        }

    }

    angular.module("app").controller("projectsController", ProjectsController);
}