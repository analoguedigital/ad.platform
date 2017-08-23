module App {
    "use strict";

    interface ICheckboxFilterController {
        activate: () => void;
    }

    interface ICheckboxFilterControllerScope extends ng.IScope {
        model: Models.IMultipleFilterValue;
    }

    class CheckboxFilterController implements ICheckboxFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: ICheckboxFilterControllerScope) {
            $scope.valueChanged = (value) => { this.optionValueChanged(value) };
            this.activate();
        }

        activate() {
            var filter = this.$scope.metricFilter;

            this.$scope.model = {
                shortTitle: filter.shortTitle,
                type: Models.FilterValueTypes.MultipleFilterValue,
                options: filter.dataList,
                values: []
            };

            this.$scope.filterValues.push(this.$scope.model);
        }

        optionValueChanged(value) {
            var selectedItems = _.filter(this.$scope.model.options, (opt) => { return opt.selected == true });
            this.$scope.model.values = selectedItems.map((item) => { return item.value });;
        }
    }

    angular.module("app").controller("checkboxFilterController", CheckboxFilterController);
}