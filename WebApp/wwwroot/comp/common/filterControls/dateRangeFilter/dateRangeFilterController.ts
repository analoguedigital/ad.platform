module App {
    "use strict";

    interface IDateRangeFilterController {
        activate: () => void;
        openStartDateCalendar: () => void;
        openEndDateCalendar: () => void;
    }

    interface IDateRangeFilterControllerScope extends ng.IScope {
        startDate: Date;
        endDate: Date;

        startDateCalendar: any;
        endDateCalendar: any;
        dateTimeFormat: string;

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

            var filterValue = {
                metricId: filter.metricId,
                shortTitle: filter.shortTitle,
                startDate: filter.startDate,
                endDate: filter.endDate
            };

            this.$scope.filterValues.push(filterValue);

            this.$scope.$watchGroup(['startDate', 'endDate'], (value) => {
                var start = value[0];
                var end = value[1];

                var filterValue: any = _.find(this.$scope.filterValues, { 'metricId': filter.metricId });
                if (filterValue) {
                    filterValue.startDate = start;
                    filterValue.endDate = end;
                }
            });
        }
    }

    angular.module("app").controller("dateRangeFilterController", DateRangeFilterController);
}