module App {
    "use strict";

    interface ITimeRangeFilterController {
        activate: () => void;
    }

    interface ITimeRangeFilterControllerScope extends ng.IScope {
        model: any;
    }

    class TimeRangeFilterController implements ITimeRangeFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: ITimeRangeFilterControllerScope) {
            $scope.model = {
                startTime: undefined,
                endTime: undefined
            };

            this.activate();
        }

        activate() {
            var filter = this.$scope.metricFilter;
            this.$scope.metricFilters.push(filter);

            var filterValue = {
                id: filter.metricId,
                shortTitle: filter.shortTitle,
                type: 'range',
                fromValue: undefined,
                toValue: undefined
            };

            this.$scope.filterValues.push(filterValue);

            this.$scope.$watchGroup(['model.startTime', 'model.endTime'], (values) => {
                var start = values[0];
                var end = values[1];

                var filterValue: any = _.find(this.$scope.filterValues, { 'id': this.$scope.metricFilter.metricId });
                if (filterValue) {
                    filterValue.fromValue = start;
                    filterValue.toValue = end;
                }
            });

            this.$scope.$on('reset-filter-controls', () => {
                this.$scope.model.startTime = undefined;
                this.$scope.model.endTime = undefined;
            });
        }
    }

    angular.module("app").controller("timeRangeFilterController", TimeRangeFilterController);
}