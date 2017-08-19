module App {
    "use strict";

    interface INumericRangeFilterController {
        activate: () => void;
    }

    interface INumericRangeFilterControllerScope extends ng.IScope {
        model: Models.IRangeFilterValue;
    }

    class NumericRangeFilterController implements INumericRangeFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: INumericRangeFilterControllerScope) {
            this.activate();
        }

        activate() {
            var filter = this.$scope.metricFilter;

            this.$scope.model = {
                id: filter.metricId,
                shortTitle: filter.shortTitle,
                type: Models.FilterValueTypes.RangeFilterValue,
                fromValue: undefined,
                toValue: undefined
            };

            this.$scope.filterValues.push(this.$scope.model);
        }
    }

    angular.module("app").controller("numericRangeFilterController", NumericRangeFilterController);
}