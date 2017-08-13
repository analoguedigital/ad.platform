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
            this.$scope.metricFilters.push(filter);
        }
    }

    angular.module("app").controller("dateRangeFilterController", DateRangeFilterController);
}