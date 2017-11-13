
((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.dashboard", {
                abstract: true,
                url: "",
                templateUrl: "comp/home/dashboard/dashboardView.html",
                controller: "dashboardController",
                ncyBreadcrumb: { label: 'Dashboard' }
            })
            .state("home.dashboard.layout", <App.Models.IAppRoute>{
                url: "/",
                views: {
                    "students": { templateUrl: "comp/home/dashboard/students/studentsView.html", controller: "studentsController" },
                    "updates": {
                        templateUrl: "comp/home/dashboard/updates/updatesView.html",
                        controller: "updatesController",
                        controllerAs: "ctrl",
                    }
                },
                ncyBreadcrumb: { label: 'Dashboard' },
                module: "private"
            })
            .state("home.studentChart", <App.Models.IAppRoute>{
                url: "/chart/:projectId",
                templateUrl: "comp/home/dashboard/students/chart/studentChartView.html",
                controller: "studentChartController",
                controllerAs: "ctrl",
                resolve: {
                    project:
                    ['$stateParams', 'projectResource',
                        ($stateParams, projectResource: App.Resources.IProjectResource) => {
                            return projectResource.get({ id: $stateParams['projectId'] }).$promise.then((data) => {
                                return data;
                            });
                        }]
                },
                ncyBreadcrumb: { label: 'Emotional and Social Development Chart' },
                module: "private"
            });
    }
})();