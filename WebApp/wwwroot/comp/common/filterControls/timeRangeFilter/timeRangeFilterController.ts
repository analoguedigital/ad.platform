module App {
    "use strict";

    interface ITimeRangeFilterController {
        activate: () => void;
    }

    interface ITimeRangeFilterControllerScope extends ng.IScope {

    }

    class TimeRangeFilterController implements ITimeRangeFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: ITimeRangeFilterControllerScope) {
            this.activate();
        }

        activate() { }
    }

    angular.module("app").controller("timeRangeFilterController", TimeRangeFilterController);
}