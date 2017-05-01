
module App {
    "use strict";

    interface IProjectsControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        projects: Models.IProject[];
        displayedProjects: Models.IProject[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        delete: (id: string) => void;
    }

    interface IProjectsController {
        activate: () => void;
        delete: (id: string) => void;
    }

    class ProjectsController implements IProjectsController {
        static $inject: string[] = ["$scope", "projectResource"];

        constructor(
            private $scope: IProjectsControllerScope,
            private projectResource: Resources.IProjectResource) {

            $scope.title = "Users";
            $scope.delete = (id) => { this.delete(id); };
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

        delete(id: string) {
            this.projectResource.delete({ id: id },
                () => { this.load(); },
                (err) => { console.log(err); });
        }
    }

    angular.module("app").controller("projectsController", ProjectsController);
}