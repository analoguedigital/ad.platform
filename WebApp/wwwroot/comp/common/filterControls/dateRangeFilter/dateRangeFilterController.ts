module App {
    "use strict";

    interface IDateRangeFilterController {
        activate: () => void;
        openStartDateCalendar: () => void;
        openEndDateCalendar: () => void;
    }

    interface IDateRangeFilterControllerScope extends ng.IScope {
        startDateCalendar: any;
        endDateCalendar: any;
        dateTimeFormat: string;
        model: any;

        openStartDateCalendar: () => void;
        openEndDateCalendar: () => void;
    }

    class DateRangeFilterController implements IDateRangeFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: IDateRangeFilterControllerScope) {
            $scope.startDateCalendar = { isOpen: false };
            $scope.endDateCalendar = { isOpen: false };

            $scope.openStartDateCalendar = () => { this.openStartDateCalendar(); }
            $scope.openEndDateCalendar = () => { this.openEndDateCalendar(); }

            $scope.dateTimeFormat = "dd/MM/yyyy";
            $scope.model = {
                startDate: undefined,
                endDate: undefined
            };

            this.activate();
        }

        openStartDateCalendar() {
            this.$scope.startDateCalendar.isOpen = true;
        }

        openEndDateCalendar() {
            this.$scope.endDateCalendar.isOpen = true;
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

            this.$scope.$watchGroup(['model.startDate', 'model.endDate'], (values) => {
                var start = values[0];
                var end = values[1];

                var filterValue: any = _.find(this.$scope.filterValues, { 'id': this.$scope.metricFilter.metricId });
                if (filterValue) {
                    filterValue.fromValue = start;
                    filterValue.toValue = end;
                }
            });

            this.$scope.$on('reset-filter-controls', () => {
                this.$scope.model.startDate = undefined;
                this.$scope.model.endDate = undefined;
            });
        }
    }

    angular.module("app").controller("dateRangeFilterController", DateRangeFilterController);
}