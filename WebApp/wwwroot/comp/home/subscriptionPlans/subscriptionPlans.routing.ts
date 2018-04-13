
((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.subscriptionplans", {
                abstract: true,
                url: "/subscription-plans",
                template: "<ui-view />"
            })
            .state("home.subscriptionplans.list", <App.Models.IAppRoute>{
                url: "",
                templateUrl: "comp/home/subscriptionPlans/subscriptionPlansView.html",
                controller: "subscriptionPlansController",
                ncyBreadcrumb: { label: 'Directory' },
                module: "private"
            })
            .state("home.subscriptionplans.edit", <App.Models.IAppRoute>{
                url: "/edit/:id",
                templateUrl: "comp/home/subscriptionPlans/edit/subscriptionPlanEditView.html",
                controller: "subscriptionPlanEditController",
                ncyBreadcrumb: { label: 'Edit', parent: 'home.subscriptionplans.list' },
                module: "private"
            });
    }
})();