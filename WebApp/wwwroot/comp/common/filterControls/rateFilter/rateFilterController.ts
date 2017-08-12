
module App {
    "use strict";

    interface IRateFilterController {
        activate: () => void;
    }

    interface IRateFilterControllerScope extends ng.IScope {
        sliderOptions: any;
    }


    class RateFilterController implements IRateFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: IRateFilterControllerScope) {
            this.activate();
        }

        activate() {
            //console.log(this.$scope.metadata);

            if (this.$scope.metadata.dataListItems.length < 1) {
                // basic slider (min/max range)
                this.$scope.sliderOptions = {
                    floor: 1,
                    ceil: this.$scope.metadata.maxValue,
                    showTicks: true
                };
            } else {
                var items = this.$scope.metadata.dataListItems;
                items.sort((a, b) => a.value - b.value);

                var steps = this.$scope.metadata.dataListItems.map((val) => {
                    return {
                        value: val.value,
                        legend: val.text
                    };
                });

                this.$scope.sliderOptions = {
                    showTicks: true,
                    showTicksValues: true,
                    stepsArray: steps
                };
            }
        }
    }

    angular.module("app").controller("rateFilterController", RateFilterController);
}