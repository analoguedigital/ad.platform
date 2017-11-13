
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
            $scope.$on('update-rate-metrics', () => { this.activate(); });
        }

        activate() {
            if (!this.$scope.metric.isAdHoc) {
                // basic slider (min/max bound)
                this.$scope.sliderOptions = {
                    floor: this.$scope.metric.minValue,
                    ceil: this.$scope.metric.maxValue,
                    showTicks: true
                };
            } else {
                // ad-hoc data list. sort values first.
                var items = this.$scope.metric.adHocItems;
                items.sort((a, b) => a.value - b.value);

                let steps = this.$scope.metric.adHocItems.map((val) => {
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

            if (_.isEmpty(this.$scope.formValues)) {
                this.$scope.formValue = this.$scope.ctrl.addFormValue(this.$scope.metric, this.$scope.dataListItem, this.$scope.rowNumber);
                this.$scope.formValue.numericValue = this.$scope.metric.defaultValue;
            }
            else {
                this.$scope.formValue = this.$scope.formValues[0];
            }

            if (this.$scope.isViewMode || this.$scope.isPrintMode) {
                this.$scope.sliderOptions.readOnly = true;
            }

        }
    }

    angular.module("app").controller("rateMetricController", RateMetricController);
}