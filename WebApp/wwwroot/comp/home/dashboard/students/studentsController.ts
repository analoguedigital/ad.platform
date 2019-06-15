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

        clientsCount: number;
        staffCount: number;
        groupsCount: number;
        totalCount: number;
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
            this.$scope.accountTypeFilter = 'clients';
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

                    this.countProjectTypes();

                    if (projects.length === 1) {
                        this.$state.go('home.projects.summary', { id: projects[0].id });
                    }
                });
            }
        }

        countProjectTypes() {
            this.$scope.clientsCount = _.filter(this.$scope.safeProjects, (p) => { return p.createdBy !== null && p.createdBy.accountType === 0; }).length;
            this.$scope.staffCount = _.filter(this.$scope.safeProjects, (p) => { return (p.createdBy === null || p.createdBy.accountType === 1) && !p.isAggregate; }).length;
            this.$scope.groupsCount = _.filter(this.$scope.safeProjects, (p) => { return p.isAggregate === true; }).length;
            this.$scope.totalCount = this.$scope.clientsCount + this.$scope.staffCount + this.$scope.groupsCount;
        }

        filterProjects(type: string) {
            this.$scope.accountTypeFilter = type;
            this.localStorageService.set('dashboard_account_type_filter', type);

            if (type === 'clients') {
                this.$scope.projects = _.filter(this.$scope.safeProjects, (p) => { return p.createdBy !== null && p.createdBy.accountType === 0; });
            } else if (type === 'staff') {
                this.$scope.projects = _.filter(this.$scope.safeProjects, (p) => { return (p.createdBy === null || p.createdBy.accountType === 1) && !p.isAggregate; });
            } else if (type === 'groups') {
                this.$scope.projects = _.filter(this.$scope.safeProjects, (p) => { return p.isAggregate === true; });
            } else if (type === 'all') {
                this.$scope.projects = this.$scope.safeProjects;
            }

            this.$scope.displayedProjects = [].concat(this.$scope.projects);
        }

    }

    angular.module("app").controller("studentsController", StudentsController);
}