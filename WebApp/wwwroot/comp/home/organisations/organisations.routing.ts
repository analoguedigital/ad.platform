
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
            .state("home.organisations.list", {
                url: "",
                templateUrl: "comp/home/organisations/organisationsView.html",
                controller: "organisationsController",
                ncyBreadcrumb: { label: 'Organisations' }
            })
            .state("home.organisations.edit", {
                url: "/edit/:id",
                templateUrl: "comp/home/organisations/edit/organisationEditView.html",
                controller: "organisationEditController",
                ncyBreadcrumb: { label: 'Edit', parent: 'home.organisations.list' }
            }) ;
    }
})();