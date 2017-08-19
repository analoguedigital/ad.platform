module App {
    "use strict";

    interface ICheckboxFilterController {
        activate: () => void;
    }

    interface ICheckboxFilterControllerScope extends ng.IScope {
        options: any[];
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

            var options = filter.dataList;
            _.forEach(options, (opt) => { opt.selected = false });

            this.$scope.options = options;

            this.$scope.model = {
                id: filter.metricId,
                shortTitle: filter.shortTitle,
                type: 'multiple',
                values: []
            };

            this.$scope.filterValues.push(this.$scope.model);

            this.$scope.$on('reset-filter-controls', () => {
                _.forEach(this.$scope.model.options, (opt) => { opt.selected = false; });
            });
        }

        optionValueChanged(value) {
            var selectedItems = _.filter(this.$scope.options, (opt: any) => { return opt.selected == true });
            var values = selectedItems.map((item) => { return item.value });

            this.$scope.model.values = values;
        }
    }

    angular.module("app").controller("checkboxFilterController", CheckboxFilterController);
}