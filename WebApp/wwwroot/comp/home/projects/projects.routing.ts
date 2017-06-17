﻿
((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.projects", {
                abstract: true,
                url: "/projects",
                template: "<ui-view />"
            })
            .state("home.projects.list", {
                url: "",
                templateUrl: "comp/home/projects/projectsView.html",
                controller: "projectsController",
                ncyBreadcrumb: { label: 'Directory' }
            })
            .state("home.projects.edit", {
                url: "/edit/:id",
                templateUrl: "comp/home/projects/edit/projectEditView.html",
                controller: "projectEditController",
                ncyBreadcrumb: { label: 'Edit', parent: 'home.projects.list' }
            })
            .state("home.projects.assignments", {
                url: "/assignments/:id",
                templateUrl: "comp/home/projects/assignments/projectAssignmentsView.html",
                controller: "projectAssignmentsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Assignments', parent: 'home.projects.list' }
            })
            .state("home.projects.summary", {
                url: "/summary/:id",
                templateUrl: "comp/home/projects/summary/projectSummaryView.html",
                controller: "projectSummaryController",
                controllerAs: "ctrl",
                resolve: {
                    project: ['$stateParams', 'projectResource',
                        ($stateParams, projectResource: App.Resources.IProjectResource) => {
                            return projectResource.get({ id: $stateParams['id'] }).$promise.then((data) => {
                                return data;
                            });
                        }]
                },
                ncyBreadcrumb: { label: 'Summary', parent: 'home.projects.list' }
            });
    }
})();