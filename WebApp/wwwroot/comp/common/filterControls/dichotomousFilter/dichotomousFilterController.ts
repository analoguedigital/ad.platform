module App {
    "use strict";

    interface IDichotomousFilterController {
        activate: () => void;
    }

    interface IDichotomousFilterControllerScope extends ng.IScope {

    }

    class DichotomousFilterController implements IDichotomousFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: IDichotomousFilterControllerScope) {
            this.activate();
        }

        activate() {
        }
    }

    angular.module("app").controller("dichotomousFilterController", DichotomousFilterController);
}