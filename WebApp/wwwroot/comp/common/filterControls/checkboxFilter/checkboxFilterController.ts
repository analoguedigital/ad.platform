module App {
    "use strict";

    interface ICheckboxFilterController {
        activate: () => void;
    }

    interface ICheckboxFilterControllerScope extends ng.IScope {

    }

    class CheckboxFilterController implements ICheckboxFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: ICheckboxFilterControllerScope) {
            this.activate();
        }

        activate() {
            var filter = this.$scope.metricFilter;
        }
    }

    angular.module("app").controller("checkboxFilterController", CheckboxFilterController);
}