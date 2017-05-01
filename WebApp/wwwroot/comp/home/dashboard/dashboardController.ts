
module App {
    "use strict";

    interface IDashboardControllerScope extends ng.IScope {
        title: string;
    }

    interface IDashboardController {
        activate: () => void;
    }

    class DashboardController implements IDashboardController {

        static $inject: string[] = ['$scope'];
        constructor(private $scope: IDashboardControllerScope) {

            $scope.title = "Dashboard";
            this.activate();
        }

        activate() { }

    }

    angular.module("app").controller("dashboardController", DashboardController);
}