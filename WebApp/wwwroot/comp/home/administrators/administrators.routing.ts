((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.administrators", {
                abstract: true,
                url: "/administrators",
                template: "<ui-view />"
            })
            .state("home.administrators.list", <App.Models.IAppRoute>{
                url: "/",
                templateUrl: "comp/home/administrators/administratorsView.html",
                controller: "administratorsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Directory of Administrators' },
                module: "private"
            })
            .state("home.administrators.editSuperUser", <App.Models.IAppRoute>{
                url: "/edit-super-user/:id",
                templateUrl: "comp/home/administrators/editSuperUser/superUserEditView.html",
                controller: "superUserEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit Super User', parent: 'home.administrators.list' },
                module: 'private'
            })
            .state("home.administrators.editPlatformUser", <App.Models.IAppRoute>{
                url: "/edit-platform-user/:id",
                templateUrl: "comp/home/administrators/editPlatformUser/platformUserEditView.html",
                controller: "platformUserEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit Platform User', parent: 'home.administrators.list' },
                module: 'private'
            });
    }
})();