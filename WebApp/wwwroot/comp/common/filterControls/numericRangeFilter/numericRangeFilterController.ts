module App {
    "use strict";

    interface INumericRangeFilterController {
        activate: () => void;
    }

    interface INumericRangeFilterControllerScope extends ng.IScope {

    }

    class NumericRangeFilterController implements INumericRangeFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: INumericRangeFilterControllerScope) {
            this.activate();
        }

        activate() {
            var filter = this.$scope.metricFilter;
            this.$scope.metricFilters.push(filter);
        }
    }

    angular.module("app").controller("numericRangeFilterController", NumericRangeFilterController);
}