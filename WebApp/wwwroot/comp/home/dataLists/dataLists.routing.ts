
((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.datalists", {
                abstract: true,
                url: "/datalists",
                template: "<ui-view />"
            })
            .state("home.datalists.list", <App.Models.IAppRoute>{
                url: "",
                templateUrl: "comp/home/datalists/dataListsView.html",
                controller: "dataListsController",
                ncyBreadcrumb: { label: 'Data Lists' },
                module: "private"
            })
            .state("home.datalists.edit", <App.Models.IAppRoute>{
                url: "/edit/:id",
                templateUrl: "comp/home/datalists/edit/dataListEditView.html",
                controller: "dataListEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit', parent: 'home.datalists.list' },
                module: "private"
            });
    }
})();