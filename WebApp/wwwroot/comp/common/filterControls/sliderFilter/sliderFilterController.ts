
module App {
    "use strict";

    interface ISliderFilterController {
        activate: () => void;
    }

    interface ISliderFilterControllerScope extends ng.IScope {
        sliderOptions: any;
    }


    class SliderFilterController implements ISliderFilterController {
        static $inject: string[] = ['$scope'];

        constructor(private $scope: ISliderFilterControllerScope) {
            this.activate();
        }

        activate() {
            var filter = this.$scope.metricFilter;

            if (filter.dataList.length < 1) {
                // basic slider (min/max range)
                this.$scope.sliderOptions = {
                    floor: 1,
                    ceil: filter.maxValue,
                    showTicks: true
                };
            } else {
                var items = filter.dataList;
                items.sort((a, b) => a.value - b.value);

                var steps = filter.dataList.map((val) => {
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

    angular.module("app").controller("sliderFilterController", SliderFilterController);
}