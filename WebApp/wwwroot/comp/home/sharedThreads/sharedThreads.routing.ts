//((): void => {
//    "use strict";

//    angular.module("app")
//        .config(ConfigRoutes);

//    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
//    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
//        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

//        $stateProvider
//            .state("home.sharedThreads", {
//                abstract: true,
//                url: "/shared-threads",
//                template: "<ui-view />"
//            })
//            .state("home.sharedThreads.list", <App.Models.IAppRoute>{
//                url: "",
//                templateUrl: "comp/home/sharedThreads/sharedThreadsView.html",
//                controller: "sharedThreadsController",
//                controllerAs: "ctrl",
//                ncyBreadcrumb: { label: 'Directory of Shared Threads' },
//                module: "private"
//            })
//            .state("home.sharedThreads.list.summary", <App.Models.IAppRoute>{
//                url: "/:projectId",
//                templateUrl: "comp/home/sharedThreads/summary/sharedThreadsSummaryView.html",
//                controller: "sharedThreadsSummaryController",
//                controllerAs: "ctrl",
//                ncyBreadcrumb: { label: 'Summary' },
//                resolve: {
//                    project:
//                    ['$stateParams', 'projectResource',
//                        ($stateParams, projectResource: App.Resources.IProjectResource) => {
//                            var projectId = $stateParams['projectId'];
//                            if (projectId == null || projectId.length < 1)
//                                return null;

//                            return projectResource.getDirect({ id: projectId }).$promise.then((data) => {
//                                return data;
//                            });
//                        }]
//                },
//                module: "private"
//            })
//            .state("home.sharedThreads.list.all", <App.Models.IAppRoute>{
//                url: "/:projectId/all/:formTemplateId",
//                templateUrl: "comp/home/sharedThreads/all/allSharedThreadsView.html",
//                controller: "allSharedThreadsController",
//                controllerAs: "ctrl",
//                resolve: {
//                    formTemplate:
//                    ['$stateParams', 'formTemplateResource',
//                        ($stateParams, formTemplateResource: App.Resources.IFormTemplateResource) => {
//                            return formTemplateResource.get({ id: $stateParams['formTemplateId'] }).$promise.then((data) => {
//                                return data;
//                            });
//                        }],
//                    project:
//                    ['$stateParams', 'projectResource',
//                        ($stateParams, projectResource: App.Resources.IProjectResource) => {
//                            return projectResource.getDirect({ id: $stateParams['projectId'] }).$promise.then((data) => {
//                                return data;
//                            });
//                        }]

//                },
//                ncyBreadcrumb: { label: 'All shared records' },
//                module: "private"
//            });
//    }
//})();