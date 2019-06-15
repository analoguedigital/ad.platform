
module App {
    "use strict";

    interface ICaseAssignmentsControllerScope extends ng.IScope {
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

        delete: (id: string) => void;
    }

    interface ICaseAssignmentsController {
        activate: () => void;
        delete: (id: string) => void;
    }

    class CaseAssignmentsController implements ICaseAssignmentsController {
        static $inject: string[] = ["$scope", "$stateParams", "projectResource", "toastr", "localStorageService"];

        constructor(
            private $scope: ICaseAssignmentsControllerScope,
            private $stateParams: ng.ui.IStateParamsService,
            private projectResource: Resources.IProjectResource,
            private toastr: any,
            private localStorageService: ng.local.storage.ILocalStorageService) {

            $scope.title = "Assignments and Permissions";
            $scope.delete = (id) => { this.delete(id); };

            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            var accountTypeFilter = <string>this.localStorageService.get('case_assignments_account_type_filter');
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

        delete(id: string) {
            this.projectResource.delete({ id: id },
                () => {
                    this.load();
                },
                (err) => {
                    console.error(err);
                    this.toastr.error(err.data.message);
                });
        }

        countProjectTypes() {
            this.$scope.clientsCount = _.filter(this.$scope.safeProjects, (p) => { return p.createdBy !== null && p.createdBy.accountType === 0; }).length;
            this.$scope.staffCount = _.filter(this.$scope.safeProjects, (p) => { return (p.createdBy === null || p.createdBy.accountType === 1) && !p.isAggregate; }).length;
            this.$scope.groupsCount = _.filter(this.$scope.safeProjects, (p) => { return p.isAggregate === true; }).length;
            this.$scope.totalCount = this.$scope.clientsCount + this.$scope.staffCount + this.$scope.groupsCount;
        }

        filterProjects(type: string) {
            this.$scope.accountTypeFilter = type;
            this.localStorageService.set('case_assignments_account_type_filter', type);

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

    angular.module("app").controller("caseAssignmentsController", CaseAssignmentsController);
}