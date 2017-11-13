module App {
    "use strict";

    interface IFilterController {
        activate: () => void;
    }

    export interface IFilterControllerScope extends ng.IScope {
        viewName: string;
    }

    class FilterController implements IFilterController {
        static $inject: string[] = ["$scope", "$state"];
        constructor(
            private $scope: IFilterControllerScope,
            private $state: angular.ui.IStateService) {

            this.activate();
        }

        activate() {
            var filter = this.$scope.metricFilter;
            var viewName = undefined;

            switch (filter.type) {
                case "Text": {
                    viewName = "textFilter";
                    break;
                }
                case "Checkbox": {
                    viewName = "checkboxFilter";
                    break;
                }
                case "DateRange": {
                    viewName = "dateRangeFilter";
                    break;
                }
                case "NumericRange": {
                    viewName = "numericRangeFilter";
                    break;
                }
                case "TimeRange": {
                    viewName = "timeRangeFilter";
                    break;
                }
            }

            this.$scope.viewName = viewName;
        }
    }

    angular.module("app").controller("filterController", FilterController);
}