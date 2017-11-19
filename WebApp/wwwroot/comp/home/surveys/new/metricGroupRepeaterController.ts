
module App {
    "use strict";

    interface IMetricGroupRepeaterController {
        activate: () => void;
    }

    interface IMetricGroupRepeaterControllerScope extends App.IMetricControllerScope {
        search: any;
        relationships: any;
        relationshipTitles: any[];
        isDataList: boolean;
        rows: any[];
        maxRow: number;
        metricGroup: Models.IMetricGroup;
        dataListItems: Models.IDataListItem[];
        allFormValues: Models.FormValue[];

        attrFilter: (search: string) => void;
        addRow: () => void;
        showAll: () => void;
        deleteRow: (rowNumber) => void;
    }

    export class MetricGroupRepeaterController implements IMetricGroupRepeaterController {

        private allText: string = "- all -";

        static $inject: string[] = ['$scope', 'dataListItemResource'];

        constructor(
            private $scope: IMetricGroupRepeaterControllerScope,
            private dataListItemResource: Resources.IDataListItemResource) {

            $scope.search = {};
            $scope.relationships = {};
            $scope.relationshipTitles = [];
            $scope.isDataList = false;
            $scope.rows = [];
            $scope.maxRow = 15;

            $scope.attrFilter = (search) => { this.attrFilter(search); };
            $scope.addRow = () => { this.addRow(); };
            $scope.showAll = () => { this.showAll(); };
            $scope.deleteRow = (rowNumber) => { this.deleteRow(rowNumber); };

            if ($scope.metricGroup.dataListId !== null && $scope.metricGroup.dataListId !== undefined) {
                this.dataListItemResource.query({ dataListId: this.$scope.metricGroup.dataListId }).$promise
                    .then((items) => {
                        this.$scope.dataListItems = items;
                        this.activate();
                    });
            }
            else {
                this.activate();
            }
        }

        activate() {
            var metricGroup = this.$scope.metricGroup;
            var survey = this.$scope.ctrl.survey;

            if (metricGroup.type === "IterativeRepeater") {
                this.$scope.isDataList = false;
                if (survey.serial == null) { // new form
                    for (var i = 0; i < metricGroup.numberOfRows; i++) {
                        this.$scope.rows.push({ rowNumber: i + 1, dataListItem: undefined });
                    }
                } else { // view/edit forms
                    var metricIds = _.map(metricGroup.metrics, (metric) => { return metric.id });
                    var formValues = _.filter(survey.formValues, (fv) => {
                        return _.includes(metricIds, fv.metricId);
                    });

                    var records = formValues.length / metricGroup.metrics.length;
                    for (var i = 0; i < records; i++) {
                        this.$scope.rows.push({ rowNumber: i + 1, dataListItem: undefined });
                    }
                }
            }
            else {
                var dataListItems = this.$scope.dataListItems;

                this.$scope.isDataList = true;
                this.$scope.relationshipTitles = _.map(dataListItems[0].attributes, 'title');
                angular.forEach(dataListItems, (item, index) => {
                    this.$scope.rows.push({ rowNumber: index + 1, dataListItem: item });

                    //angular.forEach(item.attributes, (attr) => {
                    //    if (this.$scope.relationships[attr.title] === undefined)
                    //        this.$scope.relationships[attr.title] = [this.allText];
                    //    this.$scope.relationships[attr.title].push(attr.value);
                    //});
                });

                angular.forEach(this.$scope.relationshipTitles, (title) => {
                    this.$scope.search[title] = this.allText;
                    this.$scope.relationships[title] = _.uniq(this.$scope.relationships[title]);
                    this.$scope.relationships[title] = _.sortBy(this.$scope.relationships[title], (s) => { return s; });
                });


            }
        }

        attrFilter(search) {
            var keys = Object.keys(search);
            return (item: Models.IDataListItem, index) => {
                var matched = true;
                angular.forEach(keys, (prop) => {
                    if (search[prop] === this.allText) return;
                    //if (_.find(item.attributes, 'title', prop).value != search[prop])
                    //    matched = false;
                });
                return matched;
            }
        }
        addRow() {
            this.$scope.rows.push({ rowNumber: this.$scope.rows.length + 1, dataListItem: undefined });
        }

        showAll() {
            this.$scope.maxRow += this.$scope.maxRow;
        }

        deleteRow(rowNumber) {

            this.$scope.rows.splice(rowNumber - 1, 1);

            var groupMetricIds = _.map(this.$scope.metricGroup.metrics, "id");

            _.remove(this.$scope.ctrl.survey.formValues, (formValue) => {
                return formValue.rowNumber === rowNumber && _.includes(groupMetricIds, formValue.metricId);
            });

            angular.forEach(this.$scope.allFormValues, (formValue) => {
                if (_.includes(groupMetricIds, formValue.metricId)) {
                    if (formValue.rowNumber >= rowNumber) {
                        formValue.rowNumber = formValue.rowNumber - 1;
                    }
                }
            });
        }
    }

    angular.module("app").controller("metricGroupRepeaterController", MetricGroupRepeaterController);
}