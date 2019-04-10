module App {
    "use strict";

    interface IStudentsControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        safeProjects: Models.IProject[];
        projects: Models.IProject[];
        displayedProjects: Models.IProject[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
        accountTypeFilter: string;
    }

    interface IStudentsController {
        activate: () => void;
    }

    class StudentsController implements IStudentsController {
        static $inject: string[] = ["$scope", "$state", "$timeout", "projectResource", "userContextService", "localStorageService"];
        constructor(
            private $scope: IStudentsControllerScope,
            private $state: ng.ui.IStateService,
            private $timeout: ng.ITimeoutService,
            private projectResource: Resources.IProjectResource,
            private userContextService: Services.IUserContextService,
            private localStorageService: ng.local.storage.ILocalStorageService) {

            $scope.title = "Projects";

            this.activate();
        }

        activate() {
            this.$scope.accountTypeFilter = 'all';
            this.load();
        }

        load() {
            var platformAdminRole = "Platform administrator";
            var userRoles = this.userContextService.current.user.roles;

            if (userRoles.indexOf(platformAdminRole, 0) === -1) {
                this.projectResource.query().$promise.then((projects) => {
                    this.$scope.safeProjects = projects;
                    this.$scope.projects = projects;
                    this.$scope.displayedProjects = [].concat(this.$scope.projects);

                    var accountTypeFilter = <string>this.localStorageService.get('dashboard_account_type_filter');
                    if (accountTypeFilter && accountTypeFilter.length) {
                        this.filterProjects(accountTypeFilter);
                    }

                    if (projects.length === 1) {
                        this.$state.go('home.projects.summary', { id: projects[0].id });
                    }
                });
            }
        }

        filterProjects(type: string) {
            this.$scope.accountTypeFilter = type;
            this.localStorageService.set('dashboard_account_type_filter', type);

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

    angular.module("app").controller("studentsController", StudentsController);
}