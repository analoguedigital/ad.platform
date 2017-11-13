/// <reference path="../_metric/metriccontroller.ts" />

module App {
    "use strict";

    interface IFreeTextMetricController {
        activate: () => void;
    }

    interface IFreeTextMetricControllerScope extends App.IMetricControllerScope
    {

    }

    export class FreeTextMetricController implements IFreeTextMetricController {

        static $inject: string[] = ['$scope'];

        constructor(private $scope: IFreeTextMetricControllerScope) {
            this.activate();
        }

        activate() {
            if (_.isEmpty(this.$scope.formValues)) {
                this.$scope.formValue = this.$scope.ctrl.addFormValue(this.$scope.metric, this.$scope.dataListItem, this.$scope.rowNumber);
            }
            else {
                this.$scope.formValue = this.$scope.formValues[0];
            }
        }
    }

    angular.module("app").controller("freeTextMetricController", FreeTextMetricController);
}