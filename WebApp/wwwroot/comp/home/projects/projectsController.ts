﻿module App {
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

        delete: (id: string) => void;
    }

    interface IProjectsController {
        activate: () => void;
        delete: (id: string) => void;
    }

    class ProjectsController implements IProjectsController {
        static $inject: string[] = ["$scope", "$state", "$stateParams", "$uibModal", "projectResource"];

        constructor(
            private $scope: IProjectsControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private projectResource: Resources.IProjectResource) {

            $scope.title = "Staff and Clients";
            $scope.accountTypeFilter = 'all';

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

        addNewProject() {
            this.$state.go('home.projects.edit');
        }

        delete(id: string) {
            this.projectResource.delete({ id: id },
                () => { this.load(); },
                (err) => { console.log(err); });
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

    angular.module("app").controller("projectsController", ProjectsController);
}