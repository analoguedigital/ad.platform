((): void => {
    "use strict";

    angular.module("app").config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.settings", {
                abstract: true,
                url: "/settings",
                template: "<ui-view />"
            })
            .state("home.settings.list", <App.Models.IAppRoute>{
                url: "",
                templateUrl: "comp/home/myAccount/settings/settingsView.html",
                controller: "settingsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: "Settings" },
                module: "private"
            })
            .state("home.subscriptions", {
                abstract: true,
                url: "/subscriptions",
                template: "<ui-view />"
            })
            .state("home.subscriptions.list", <App.Models.IAppRoute>{
                url: "",
                templateUrl: "comp/home/myAccount/subscriptionsView.html",
                controller: "subscriptionsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Subscriptions and Payments' },
                module: "public"
            });
    }
})();