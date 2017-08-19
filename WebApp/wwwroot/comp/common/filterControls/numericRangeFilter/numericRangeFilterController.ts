module App {
    "use strict";

    interface INumericRangeFilterController {
        activate: () => void;
    }

    interface INumericRangeFilterControllerScope extends ng.IScope {
        model: any;
    }

    class NumericRangeFilterController implements INumericRangeFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: INumericRangeFilterControllerScope) {
            $scope.model = {
                startValue: undefined,
                endValue: undefined
            };

            this.activate();
        }

        activate() {
            var filter = this.$scope.metricFilter;

            this.$scope.model = {
                id: filter.metricId,
                shortTitle: filter.shortTitle,
                type: 'range',
                fromValue: undefined,
                toValue: undefined
            };

            this.$scope.filterValues.push(this.$scope.model);

            this.$scope.$on('reset-filter-controls', () => {
                this.$scope.model.startValue = undefined;
                this.$scope.model.endValue = undefined;
            });
        }
    }

    angular.module("app").controller("numericRangeFilterController", NumericRangeFilterController);
}