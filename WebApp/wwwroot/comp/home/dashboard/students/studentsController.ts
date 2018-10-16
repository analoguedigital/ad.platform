module App {
    "use strict";

    interface IStudentsControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        projects: Models.IProject[];
        displayedProjects: Models.IProject[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
    }

    interface IStudentsController {
        activate: () => void;
    }

    class StudentsController implements IStudentsController {
        static $inject: string[] = ["$scope", "$state", "$timeout", "projectResource"];
        constructor(
            private $scope: IStudentsControllerScope,
            private $state: ng.ui.IStateService,
            private $timeout: ng.ITimeoutService,
            private projectResource: Resources.IProjectResource) {

            $scope.title = "Projects";

            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            this.projectResource.query().$promise.then((projects) => {
                this.$scope.projects = projects;
                this.$scope.displayedProjects = [].concat(this.$scope.projects);

                if (projects.length === 1) {
                    this.$state.go('home.projects.summary', { id: projects[0].id });
                }
            });
        }

    }

    angular.module("app").controller("studentsController", StudentsController);
}