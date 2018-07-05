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
        projects: Models.IProject[];
        displayedProjects: Models.IProject[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
        minicolorSettings: any;
        model: IAddAdviceThreadModel;

        delete: (id: string) => void;
    }

    interface IProjectsController {
        activate: () => void;
        delete: (id: string) => void;
    }

    class ProjectsController implements IProjectsController {
        static $inject: string[] = ["$scope", "$stateParams", "$uibModal", "projectResource"];

        constructor(
            private $scope: IProjectsControllerScope,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private projectResource: Resources.IProjectResource) {

            $scope.title = "Cases";
            $scope.delete = (id) => { this.delete(id); };

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

        delete(id: string) {
            this.projectResource.delete({ id: id },
                () => { this.load(); },
                (err) => { console.log(err); });
        }
    }

    angular.module("app").controller("projectsController", ProjectsController);
}