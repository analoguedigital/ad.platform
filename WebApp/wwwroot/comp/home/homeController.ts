
module App {
    "use strict";

    interface IHomeControllerScope extends ng.IScope {
        title: string;
    }

    interface IHomeController {
        activate: () => void;
    }

    class HomeController implements IHomeController {
        static $inject: string[] = ["$scope"];

        constructor(private $scope: IHomeControllerScope) {
            $scope.title = "home";

            this.activate();
        }

        activate() {
            //
        }
    }

    angular.module("app").controller("homeController", HomeController);
}