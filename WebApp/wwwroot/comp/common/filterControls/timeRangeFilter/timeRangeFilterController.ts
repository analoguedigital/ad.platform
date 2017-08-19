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

            this.$scope.model = {
                id: filter.metricId,
                shortTitle: filter.shortTitle,
                type: 'range',
                fromValue: undefined,
                toValue: undefined
            };

            this.$scope.filterValues.push(this.$scope.model);

            this.$scope.$on('reset-filter-controls', () => {
                this.$scope.model.startTime = undefined;
                this.$scope.model.endTime = undefined;
            });
        }
    }

    angular.module("app").controller("timeRangeFilterController", TimeRangeFilterController);
}