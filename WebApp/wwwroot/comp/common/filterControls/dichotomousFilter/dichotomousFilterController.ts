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
            var filter = this.$scope.metricFilter;
            this.$scope.metricFilters.push(filter);
        }
    }

    angular.module("app").controller("dichotomousFilterController", DichotomousFilterController);
}