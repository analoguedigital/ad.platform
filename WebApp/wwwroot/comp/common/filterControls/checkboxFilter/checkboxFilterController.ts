module App {
    "use strict";

    interface ICheckboxFilterController {
        activate: () => void;
    }

    interface ICheckboxFilterControllerScope extends ng.IScope {
        model: any;
    }

    class CheckboxFilterController implements ICheckboxFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: ICheckboxFilterControllerScope) {
            $scope.valueChanged = (value) => { this.optionValueChanged(value) };
            this.activate();
        }

        activate() {
            var filter = this.$scope.metricFilter;
            this.$scope.metricFilters.push(filter);

            var options = filter.dataList;
            _.forEach(options, (opt) => { opt.selected = false });

            this.$scope.model = {
                options: options
            };

            var filterValue = {
                shortTitle: filter.shortTitle,
                type: 'multiple',
                values: []
            };

            this.$scope.filterValues.push(filterValue);

            this.$scope.$on('reset-filter-controls', () => {
                _.forEach(this.$scope.model.options, (opt) => { opt.selected = false; });
            });
        }

        optionValueChanged(value) {
            var values = [];

            var selectedItems = _.filter(this.$scope.model.options, (opt: any) => { return opt.selected == true });

            if (selectedItems.length) {
                var sampleItem = selectedItems[0];
                if (sampleItem.id && sampleItem.id.length) {
                    values = selectedItems.map((item) => { return item.id });
                } else {
                    values = selectedItems.map((item) => { return item.value });
                }
            }

            var filterValue: any = _.find(this.$scope.filterValues, { 'shortTitle': this.$scope.metricFilter.shortTitle });
            if (filterValue) {
                filterValue.values = values;
            }
        }
    }

    angular.module("app").controller("checkboxFilterController", CheckboxFilterController);
}