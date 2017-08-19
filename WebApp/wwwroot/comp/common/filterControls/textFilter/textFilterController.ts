module App {
    "use strict";

    interface ITextFilterController {
        activate: () => void;
    }

    interface ITextFilterControllerScope extends ng.IScope {
        model: Models.ISingleFilterValue;
    }

    class TextFilterController implements ITextFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: ITextFilterControllerScope) {
            this.activate();
        }

        activate() {
            var filter = this.$scope.metricFilter;

            this.$scope.model = {
                id: filter.metricId,
                type: Models.FilterValueTypes.SingleFilterValue,
                shortTitle: filter.shortTitle,
                value: undefined
            };

            this.$scope.filterValues.push(this.$scope.model);
        }
    }

    angular.module("app").controller("textFilterController", TextFilterController);
}