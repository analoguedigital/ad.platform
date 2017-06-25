((): void => {
    "use strict";

    angular.module("app").config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.subscriptions", {
                abstract: true,
                url: "/subscriptions",
                template: "<ui-view />"
            })
            .state("home.subscriptions.list", {
                url: "",
                templateUrl: "comp/home/myAccount/subscriptionsView.html",
                controller: "subscriptionsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Subscriptions and Payments' }
            });
    }
})();