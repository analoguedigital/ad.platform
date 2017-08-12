
module App {
    "use strict";

    interface IDateFilterController {
        activate: () => void;
        openStartDateCalendar: () => void;
        openEndDateCalendar: () => void;
    }

    interface IDateFilterControllerScope extends ng.IScope {
        startDate: Date;
        endDate: Date;

        startDateCalendar: any;
        endDateCalendar: any;
        dateTimeFormat: string;

        openStartDateCalendar: () => void;
        openEndDateCalendar: () => void;
    }


    class DateFilterController implements IDateFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: IDateFilterControllerScope) {
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
            var filterValue = {
                metricId: this.$scope.metadata.metricId,
                shortTitle: this.$scope.metadata.shortTitle,
                startDate: this.$scope.startDate,
                endDate: this.$scope.endDate
            };

            this.$scope.filterValues.push(filterValue);

            this.$scope.$watchGroup(['startDate', 'endDate'], (value) => {
                var start = value[0];
                var end = value[1];

                var filterValue: any = _.find(this.$scope.filterValues, { 'metricId': this.$scope.metadata.metricId });
                if (filterValue) {
                    filterValue.startDate = start;
                    filterValue.endDate = end;
                }
            });
        }
    }

    angular.module("app").controller("dateFilterController", DateFilterController);
}