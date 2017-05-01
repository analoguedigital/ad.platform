
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
        static $inject: string[] = ["$scope", "projectResource"];

        constructor(
            private $scope: IStudentsControllerScope,
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
            });
        }
    }

    angular.module("app").controller("studentsController", StudentsController);
}