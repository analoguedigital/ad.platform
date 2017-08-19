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

            this.$scope.model = {
                id: filter.metricId,
                shortTitle: filter.shortTitle,
                type: 'multiple',
                options: options,
                values: []
            };

            this.$scope.filterValues.push(this.$scope.model);
        }

        optionValueChanged(value) {
            var selectedItems = _.filter(this.$scope.model.options, (opt: any) => { return opt.selected == true });
            this.$scope.model.values = selectedItems.map((item) => { return item.value });;
        }
    }

    angular.module("app").controller("checkboxFilterController", CheckboxFilterController);
}