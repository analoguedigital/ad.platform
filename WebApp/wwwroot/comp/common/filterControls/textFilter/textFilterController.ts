module App {
    "use strict";

    interface ITextFilterController {
        activate: () => void;
    }

    interface ITextFilterControllerScope extends ng.IScope {
        model: any;
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
                type: 'single',
                shortTitle: filter.shortTitle,
                value: ''
            };

            this.$scope.filterValues.push(this.$scope.model);

            this.$scope.$on('reset-filter-controls', () => {
                this.$scope.model.currentValue = '';
            });
        }
    }

    angular.module("app").controller("textFilterController", TextFilterController);
}