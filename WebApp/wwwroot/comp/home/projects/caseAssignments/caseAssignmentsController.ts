
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

        delete: (id: string) => void;
    }

    interface ICaseAssignmentsController {
        activate: () => void;
        delete: (id: string) => void;
    }

    class CaseAssignmentsController implements ICaseAssignmentsController {
        static $inject: string[] = ["$scope", "$stateParams", "projectResource", "toastr"];

        constructor(
            private $scope: ICaseAssignmentsControllerScope,
            private $stateParams: ng.ui.IStateParamsService,
            private projectResource: Resources.IProjectResource,
            private toastr: any) {

            $scope.title = "Assignments and Permissions";
            $scope.delete = (id) => { this.delete(id); };

            this.activate();
        }

        activate() {
            this.$scope.accountTypeFilter = 'all';
            this.load();
        }

        load() {
            var organisationId = this.$stateParams["organisationId"];
            if (organisationId === '') {
                this.projectResource.query().$promise.then((projects) => {
                    this.$scope.safeProjects = projects;
                    this.$scope.projects = projects;
                    this.$scope.displayedProjects = [].concat(this.$scope.projects);
                });
            } else {
                this.projectResource.query({ organisationId: organisationId }).$promise.then((projects) => {
                    this.$scope.safeProjects = projects;
                    this.$scope.projects = projects;
                    this.$scope.displayedProjects = [].concat(this.$scope.projects);
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

        filterProjects(type: string) {
            this.$scope.accountTypeFilter = type;

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

    angular.module("app").controller("caseAssignmentsController", CaseAssignmentsController);
}