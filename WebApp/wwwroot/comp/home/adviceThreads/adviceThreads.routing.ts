
((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.adviceThreads", {
                abstract: true,
                url: "/advice-threads",
                template: "<ui-view />"
            })
            .state("home.adviceThreads.list", <App.Models.IAppRoute>{
                url: "",
                templateUrl: "comp/home/adviceThreads/adviceThreadsView.html",
                controller: "adviceThreadsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Directory of Advice Threads' },
                module: "private"
            })
            .state("home.adviceThreads.list.summary", <App.Models.IAppRoute>{
                url: "/:projectId",
                templateUrl: "comp/home/adviceThreads/summary/adviceThreadsSummaryView.html",
                controller: "adviceThreadsSummaryController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Summary' },
                resolve: {
                    project:
                    ['$stateParams', 'projectResource',
                        ($stateParams, projectResource: App.Resources.IProjectResource) => {
                            var projectId = $stateParams['projectId'];
                            if (projectId == null || projectId.length < 1)
                                return null;

                            return projectResource.get({ id: projectId }).$promise.then((data) => {
                                return data;
                            });
                        }]
                },
                module: "private"
            })
            .state("home.adviceThreads.list.all", <App.Models.IAppRoute>{
                url: "/:projectId/all/:formTemplateId",
                templateUrl: "comp/home/adviceThreads/all/allAdviceThreadsView.html",
                controller: "allAdviceThreadsController",
                controllerAs: "ctrl",
                resolve: {
                    formTemplate:
                    ['$stateParams', 'formTemplateResource',
                        ($stateParams, formTemplateResource: App.Resources.IFormTemplateResource) => {
                            return formTemplateResource.get({ id: $stateParams['formTemplateId'] }).$promise.then((data) => {
                                return data;
                            });
                        }],
                    project:
                    ['$stateParams', 'projectResource',
                        ($stateParams, projectResource: App.Resources.IProjectResource) => {
                            return projectResource.get({ id: $stateParams['projectId'] }).$promise.then((data) => {
                                return data;
                            });
                        }]

                },
                ncyBreadcrumb: { label: 'All advice records' },
                module: "private"
            });
    }
})();