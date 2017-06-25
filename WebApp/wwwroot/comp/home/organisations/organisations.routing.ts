
((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.organisations", {
                abstract: true,
                url: "/organisations",
                template: "<ui-view />"
            })
            .state("home.organisations.list", <App.Models.IAppRoute>{
                url: "",
                templateUrl: "comp/home/organisations/organisationsView.html",
                controller: "organisationsController",
                ncyBreadcrumb: { label: 'Organisations' },
                module: "private"
            })
            .state("home.organisations.edit", <App.Models.IAppRoute>{
                url: "/edit/:id",
                templateUrl: "comp/home/organisations/edit/organisationEditView.html",
                controller: "organisationEditController",
                ncyBreadcrumb: { label: 'Edit', parent: 'home.organisations.list' },
                module: "private"
            }) ;
    }
})();