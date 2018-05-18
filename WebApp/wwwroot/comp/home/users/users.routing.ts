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
            .state("home.users.list", <App.Models.IAppRoute>{
                url: "/:organisationId",
                templateUrl: "comp/home/users/usersView.html",
                controller: "usersController",
                ncyBreadcrumb: { label: 'Staff Members' },
                module: "private"
            })
            .state("home.users.mobile", <App.Models.IAppRoute>{
                url: "/mobile/:organisationId",
                templateUrl: "comp/home/users/mobile/mobileUsersView.html",
                controller: "mobileUsersController",
                ncyBreadcrumb: { label: 'Mobile Users' },
                module: 'private'
            })
            .state("home.users.edit", <App.Models.IAppRoute>{
                url: "/edit/:accountType/:id",
                templateUrl: "comp/home/users/edit/userEditView.html",
                controller: "userEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit User', parent: '{{parentBreadcrumb}}' },
                module: "private"
            })
            .state("home.users.editSuperUser", <App.Models.IAppRoute>{
                url: "/edit-super/:id",
                templateUrl: "comp/home/users/editSuperUser/superUserEditView.html",
                controller: "superUserEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit User', parent: 'home.users.list' },
                module: 'private'
            });
    }
})();