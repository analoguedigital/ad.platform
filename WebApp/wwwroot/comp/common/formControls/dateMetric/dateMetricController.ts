
module App {
    "use strict";

    interface IDateMetricController {
        activate: () => void;
        openCalendar: () => void;
    }

    interface IDateMetricContrllerScope extends IMetricControllerScope {
        metric: Models.IDateMetric;
        metricGroup: Models.IMetricGroup;

        calendar: any;
        isPersian: boolean;
        dateTimeFormat: string;

        openCalendar: () => void;
    }


    class DateMetricController implements IDateMetricController {

        static $inject: string[] = ['$scope'];

        constructor(private $scope: IDateMetricContrllerScope) {
            $scope.calendar = { isOpen: false };
            $scope.openCalendar = () => { this.openCalendar(); }
            $scope.dateTimeFormat = "MM/dd/yyyy";

            this.activate();
        }

        openCalendar() {
            this.$scope.calendar.isOpen = true;
        }

        activate() {
            if (_.isEmpty(this.$scope.formValues)) {
                this.$scope.formValue = this.$scope.ctrl.addFormValue(this.$scope.metric, this.$scope.dataListItem, this.$scope.rowNumber);
            }
            else {
                this.$scope.formValue = this.$scope.formValues[0];
            }

            if (this.$scope.metric.hasTimeValue)
                this.$scope.dateTimeFormat = "MM/dd/yyyy HH:mm a";  
        }
    }

    angular.module("app").controller("dateMetricController", DateMetricController);
}