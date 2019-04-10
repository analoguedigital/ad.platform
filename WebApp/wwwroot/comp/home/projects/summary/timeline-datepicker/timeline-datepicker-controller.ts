module App {
    "use strict";

    interface ITimelineDatepickerModel {
        month: string;
        year: number;
    }

    interface ITimelineDatepickerController {
        currentDate: Date;
        model: ITimelineDatepickerModel;

        activate: () => void;
        close: () => void;
    }

    class TimelineDatepickerController implements ITimelineDatepickerController {
        currentDate: Date;
        model: ITimelineDatepickerModel;

        months: string[];
        years: number[];

        static $inject: string[] = ["$uibModalInstance", "toastr", "currentTimelineDate"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            public toastr: any,
            public currentTimelineDate: Date) {
            this.activate();
        }

        activate() {
            this.currentDate = this.currentTimelineDate;
            this.months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            this.years = [];

            var currentYear = this.currentDate.getFullYear();
            for (var i = currentYear - 10; i < currentYear; i++) {
                this.years.push(i);
            }

            this.years.push(currentYear);

            for (var i = currentYear + 1; i < currentYear + 11; i++) {
                this.years.push(i);
            }

            this.model = {
                month: this.months[this.currentDate.getMonth()],
                year: this.currentDate.getFullYear()
            };
        }

        changeDate() {
            var monthIndex = this.months.indexOf(this.model.month);

            var result = {
                month: monthIndex,
                year: this.model.year
            };

            this.$uibModalInstance.close(result);
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("timelineDatepickerController", TimelineDatepickerController);
}