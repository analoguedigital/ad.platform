
module App {
    "use strict";

    interface ICaseAssignmentsControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        projects: Models.IProject[];
        displayedProjects: Models.IProject[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;

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
            this.load();
        }

        load() {
            var organisationId = this.$stateParams["organisationId"];
            if (organisationId === '') {
                this.projectResource.query().$promise.then((projects) => {
                    this.$scope.projects = projects;
                    this.$scope.displayedProjects = [].concat(this.$scope.projects);
                });
            } else {
                this.projectResource.query({ organisationId: organisationId }).$promise.then((projects) => {
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
    }

    angular.module("app").controller("caseAssignmentsController", CaseAssignmentsController);
}