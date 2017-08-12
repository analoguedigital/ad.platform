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
            var parentScope = this.$scope.$parent;
            var metadata = parentScope.metadata;

            switch (metadata.inputType) {
                case "Date": {
                    this.$scope.viewName = "dateFilter";
                    break;
                }
                case "Rate": {
                    this.$scope.viewName = "rateFilter";
                    break;
                }
            }
        }
    }

    angular.module("app").controller("filterController", FilterController);
}