
module App {
    "use strict";

    interface IDashboardControllerScope extends ng.IScope {
        title: string;
    }

    interface IDashboardController {
        activate: () => void;
    }

    class DashboardController implements IDashboardController {

        static $inject: string[] = ['$scope', 'userContextService'];
        constructor(
            private $scope: IDashboardControllerScope,
            private userContextService: Services.IUserContextService) {

            $scope.title = "Summaries";
            this.activate();
        }

        activate() { }

    }

    angular.module("app").controller("dashboardController", DashboardController);
}