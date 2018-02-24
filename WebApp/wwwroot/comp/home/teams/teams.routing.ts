((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.teams", {
                abstract: true,
                url: "/teams",
                template: "<ui-view />"
            })
            .state("home.teams.list", <App.Models.IAppRoute>{
                url: "/:organisationId",
                templateUrl: "comp/home/teams/teamsView.html",
                controller: "teamsController",
                ncyBreadcrumb: { label: 'Organisation Teams' },
                module: "private"
            })
            .state("home.teams.edit", <App.Models.IAppRoute>{
                url: "/edit/:id",
                templateUrl: "comp/home/teams/edit/teamEditView.html",
                controller: "organisationTeamEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit', parent: 'home.teams.list' },
                module: "private"
            })
            .state("home.teams.assign", <App.Models.IAppRoute>{
                url: "/assign/:id",
                templateUrl: "comp/home/teams/assign/teamAssignView.html",
                controller: "organisationTeamAssignController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Assign', parent: 'home.teams.list' },
                module: 'private'
            })
            .state("home.teams.manage", <App.Models.IAppRoute>{
                url: "/manage/:id",
                templateUrl: "comp/home/teams/manage/teamManageView.html",
                controller: "organisationTeamManageController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Manage', parent: 'home.teams.list' },
                module: 'private'
            });
    }
})();