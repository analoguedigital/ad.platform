module App {
    "use strict";

    interface ITextFilterController {
        activate: () => void;
    }

    interface ITextFilterControllerScope extends ng.IScope {

    }

    class TextFilterController implements ITextFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: ITextFilterControllerScope) {
            this.activate();
        }

        activate() {
            var filter = this.$scope.metricFilter;
            this.$scope.metricFilters.push(filter);
        }
    }

    angular.module("app").controller("textFilterController", TextFilterController);
}