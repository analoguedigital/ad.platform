((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("downloads", <App.Models.IAppRoute>{
                url: "/downloads/:id",
                templateUrl: "comp/downloads/downloadsView.html",
                controller: "downloadsController",
                controllerAs: 'ctrl',
                ncyBreadcrumb: { label: 'Download Center' },
                params: { authenticationRequired: true },
                module: "private"
            });
    }
})();