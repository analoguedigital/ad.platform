module App {
    "use strict";

    interface ITimeRangeFilterController {
        activate: () => void;
    }

    interface ITimeRangeFilterControllerScope extends ng.IScope {
        model: Models.IRangeFilterValue;
    }

    class TimeRangeFilterController implements ITimeRangeFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: ITimeRangeFilterControllerScope) {
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

    angular.module("app").controller("timeRangeFilterController", TimeRangeFilterController);
}