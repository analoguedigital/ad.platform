
((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.users", {
                abstract: true,
                url: "/users",
                template: "<ui-view />"
            })
            .state("home.users.list", {
                url: "",
                templateUrl: "comp/home/users/usersView.html",
                controller: "usersController",
                ncyBreadcrumb: { label: 'Users' }
            })
            .state("home.users.edit", {
                url: "/edit/:id",
                templateUrl: "comp/home/users/edit/userEditView.html",
                controller: "userEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit', parent: 'home.users.list' }
            });
    }
})();