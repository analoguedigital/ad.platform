
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
            .state("home.caseManagement", {
                abstract: true,
                url: "/case-management",
                template: "<ui-view />"
            })
            .state("home.projects.list", <App.Models.IAppRoute>{
                url: "/:organisationId",
                templateUrl: "comp/home/projects/projectsView.html",
                controller: "projectsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Directory' },
                module: "private"
            })
            .state("home.caseManagement.caseAssignments", <App.Models.IAppRoute>{
                url: "/case-assignments/:organisationId",
                templateUrl: "comp/home/projects/caseAssignments/caseAssignmentsView.html",
                controller: "caseAssignmentsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Directory of cases' },
                module: 'private'
            })
            .state("home.caseManagement.threadAssignments", <App.Models.IAppRoute>{
                url: "/thread-assignments",
                templateUrl: "comp/home/projects/threadAssignments/threadAssignmentsView.html",
                controller: "threadAssignmentsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Directory of threads' },
                module: 'private'
            })
            .state("home.projects.edit", <App.Models.IAppRoute>{
                url: "/edit/:id",
                templateUrl: "comp/home/projects/edit/projectEditView.html",
                controller: "projectEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit', parent: 'home.projects.list' },
                module: "private"
            })
            .state("home.projects.assignments", <App.Models.IAppRoute>{
                url: "/assignments/:id",
                templateUrl: "comp/home/projects/assignments/projectAssignmentsView.html",
                controller: "projectAssignmentsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Assignments', parent: 'home.caseManagement.caseAssignments' },
                module: "private"
            })
            .state("home.projects.summary", <App.Models.IAppRoute>{
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
                ncyBreadcrumb: { label: 'Summary', parent: 'home.projects.list' },
                module: "private"
            })

            .state("projectSummaryPrintMaster", {
                abstract: true,
                url: "/projects/summary/print",
                template: "<ui-view/>"
            })

            .state("home.projects.summaryPrint", {
                parent:"projectSummaryPrintMaster",
                url: "/:sessionId",
                templateUrl: "comp/home/projects/summary/print/projectSummaryPrintView.html",
                controller: "projectSummaryPrintController",
                controllerAs: "ctrl",
                resolve: {
                    session: ['$stateParams','projectSummaryPrintSessionResource',
                        ($stateParams, projectSummaryPrintSessionResource: App.Resources.IProjectSummaryPrintSessionResource) => {
                            return projectSummaryPrintSessionResource.get({ id: $stateParams['sessionId'] }).$promise.then((data) => {
                                return data;
                            });
                        }]
                },
                ncyBreadcrumb: { label: '', parent: 'home.projects.summary' }
            });
    }
})();