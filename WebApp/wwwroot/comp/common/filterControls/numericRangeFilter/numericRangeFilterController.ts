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
            this.$scope.metricFilters.push(filter);

            var filterValue = {
                shortTitle: filter.shortTitle,
                type: 'range',
                fromValue: undefined,
                toValue: undefined
            };

            this.$scope.filterValues.push(filterValue);

            this.$scope.$watchGroup(['model.startValue', 'model.endValue'], (values) => {
                var start = values[0];
                var end = values[1];

                var filterValue: any = _.find(this.$scope.filterValues, { 'shortTitle': this.$scope.metricFilter.shortTitle });
                if (filterValue) {
                    filterValue.fromValue = start;
                    filterValue.toValue = end;
                }
            });

            this.$scope.$on('reset-filter-controls', () => {
                this.$scope.model.startValue = undefined;
                this.$scope.model.endValue = undefined;
            });
        }
    }

    angular.module("app").controller("numericRangeFilterController", NumericRangeFilterController);
}