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

            this.$scope.metricFilters.push(filter);

            var filterValue = {
                id: filter.metricId,
                type: 'single',
                shortTitle: filter.shortTitle,
                value: ''
            };

            this.$scope.filterValues.push(filterValue);

            this.$scope.model = {
                currentValue: ''
            };

            this.$scope.$watch('model.currentValue', _.debounce((value) => {
                var filterValue: any = _.find(this.$scope.filterValues, { 'id': this.$scope.metricFilter.metricId });
                if (filterValue) {
                    filterValue.value = value;
                }
            }, 1000));

            this.$scope.$on('reset-filter-controls', () => {
                this.$scope.model.currentValue = '';
            });
        }
    }

    angular.module("app").controller("textFilterController", TextFilterController);
}