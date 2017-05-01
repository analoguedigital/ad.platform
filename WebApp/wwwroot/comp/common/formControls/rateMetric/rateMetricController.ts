
module App {
    "use strict";

    interface IRateMetricController {
        activate: () => void;
    }

    interface IRateMetricControllerScope extends IMetricControllerScope {
        metric: Models.IRateMetric;
        metricGroup: Models.IMetricGroup;
        sliderOptions: any;
    }

    class RateMetricController implements IRateMetricController {

        static $inject: string[] = ['$scope'];

        constructor(private $scope: IRateMetricControllerScope) {
            this.activate();
            $scope.$watch('metric', () => { this.activate(); });
        }
        
        activate() {
            this.$scope.sliderOptions = {
                floor: 1,
                ceil: this.$scope.metric.maxValue,
                showTicks: true,
            };

            if (_.isEmpty(this.$scope.formValues)) {
                this.$scope.formValue = this.$scope.ctrl.addFormValue(this.$scope.metric, this.$scope.dataListItem, this.$scope.rowNumber);
                this.$scope.formValue.numericValue = 0;
            }
            else {
                this.$scope.formValue = this.$scope.formValues[0];
            }
        }
    }

    angular.module("app").controller("rateMetricController", RateMetricController);
}