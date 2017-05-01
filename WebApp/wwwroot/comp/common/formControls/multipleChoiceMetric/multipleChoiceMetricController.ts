
module App {
    "use strict";

    interface IMultipleChoiceMetricController {
        activate: () => void;
    }

    interface IMultipleChoiceMetricControllerScope extends IMetricControllerScope {
        metric: Models.IMultipleChoiceMetric;
        isMultipleAnswer: boolean;
        items: Models.IDataListItem[];
        getText: (id: string) => string;
    }

    class MultipleChoiceMetricController implements IMultipleChoiceMetricController {

        static $inject: string[] = ['$scope', 'dataListItemResource'];
        constructor(private $scope: IMultipleChoiceMetricControllerScope, private dataListItemResource: Resources.IDataListItemResource) {

          //  $scope.$watch('metric', () => { this.activate(); }, true);
            $scope.getText = (id) => { return this.getText(id); };

        }

        activate() {
            this.$scope.isMultipleAnswer = this.$scope.metric.viewType === "CheckBoxList";

            if (this.$scope.metric.isAdHoc) {
                this.$scope.items = _.filter(this.$scope.metric.adHocItems, { 'isDeleted': false });
                this.initiateFormValues();
            }
            else {

                this.dataListItemResource.query({ dataListId: this.$scope.metric.dataListId }).$promise
                    .then((items) => {
                        this.$scope.items = items;
                        this.initiateFormValues();
                    });
            }
        }

        initiateFormValues() {
            if (_.isEmpty(this.$scope.formValues)) {
                this.$scope.formValues = [];
            }

            if (this.$scope.isMultipleAnswer) {

                angular.forEach(this.$scope.items, (item) => {
                    if (_.find(this.$scope.formValues, { 'guidValue': item.id }) === undefined) {
                        var newValue = this.$scope.ctrl.addFormValue(this.$scope.metric, this.$scope.dataListItem, this.$scope.rowNumber);
                        newValue.guidValue = item.id;
                        this.$scope.formValues.push(newValue);
                    }
                });

                this.$scope.formValues = _.sortBy(this.$scope.formValues, (o) => { return _.find(this.$scope.items, { 'id': o.guidValue }).order });
            }
            else {
                if (_.isEmpty(this.$scope.formValues)) {
                    this.$scope.formValue = this.$scope.ctrl.addFormValue(this.$scope.metric, this.$scope.dataListItem, this.$scope.rowNumber);
                }
                else {
                    this.$scope.formValue = this.$scope.formValues[0];
                }
            }

        }

        getText(dataListItemId) {
            var item = _.find(this.$scope.items, { 'id': dataListItemId });

            if (item === undefined || item.isDeleted) return '';
            return item.text;
        };
    }

    angular.module("app").controller("multipleChoiceMetricController", MultipleChoiceMetricController);
}