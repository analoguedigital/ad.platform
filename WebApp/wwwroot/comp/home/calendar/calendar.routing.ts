((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.calendar", <App.Models.IAppRoute>{
                url: "/calendar",
                templateUrl: "comp/home/calendar/calendarView.html",
                controller: "calendarController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Calendar' },
                module: "private"
            });
    }
})();