((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.stats", <App.Models.IAppRoute>{
                url: "/stats",
                templateUrl: "comp/home/statistics/statisticsView.html",
                controller: "statisticsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Statistics' },
                module: "private"
            });
    }
})();