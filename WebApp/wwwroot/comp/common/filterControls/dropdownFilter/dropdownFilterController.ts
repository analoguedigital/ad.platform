module App {
    "use strict";

    interface IDropdownFilterController {
        activate: () => void;
    }

    interface IDropdownFilterControllerScope extends ng.IScope {

    }

    class DropdownFilterController implements IDropdownFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: IDropdownFilterControllerScope) {
            this.activate();
        }

        activate() {
            var filter = this.$scope.metricFilter;
            this.$scope.metricFilters.push(filter);
        }
    }

    angular.module("app").controller("dropdownFilterController", DropdownFilterController);
}