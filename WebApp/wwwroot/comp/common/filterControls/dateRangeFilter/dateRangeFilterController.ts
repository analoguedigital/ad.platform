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
        model: Models.IRangeFilterValue;

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

    angular.module("app").controller("dateRangeFilterController", DateRangeFilterController);
}