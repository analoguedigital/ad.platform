
module App {
    "use strict";

    interface IFormDesignController {
        title: string;
        activate: () => void;
    }

    interface Error {
        key: string;
        value: string;
    }

    class FormDesignController implements IFormDesignController {
        title: string = "Form designer";
        formId: string;
        formTemplate: Models.IFormTemplate;
        survey: Models.ISurvey;
        pages: _.Dictionary<Models.IMetricGroup[]>;
        pageNumbers: string[];
        errors: Error[];

        draggableOptions = {
            connectWith: ".metrics",
            stop: (e, ui) => {
                // if the element is removed from the first container
                if (ui.item.sortable.source.hasClass('controls') &&
                    ui.item.sortable.droptarget &&
                    ui.item.sortable.droptarget != ui.item.sortable.source &&
                    ui.item.sortable.droptarget.hasClass('metrics')) {

                    var m = ui.item.sortable.model;
                    // restore the removed item
                    ui.item.sortable.sourceModel.push(_.cloneDeep(ui.item.sortable.model));

                    var modalInstance = this.$uibModal.open({
                        animation: true,
                        templateUrl: 'comp/home/formtemplates/design/forms/' + _.camelCase(m.type) + '/enter.html',
                        controller: _.camelCase(m.type) + 'FormController',
                        size: 'lg',
                        resolve: {
                            metric: () => {
                                return m;
                            }
                        }
                    });

                    modalInstance.closed.then(() => {
                        this.formTemplate = this.formTemplate;
                    });

                }
            }
        };

        groupsDraggableOptions = {
            connectWith: ".metric-groups",
            stop: (e, ui) => {
                // if the element is removed from the first container
                if (ui.item.sortable.source.hasClass('controls') &&
                    ui.item.sortable.droptarget &&
                    ui.item.sortable.droptarget != ui.item.sortable.source &&
                    ui.item.sortable.droptarget.hasClass('metric-groups')) {

                    var g = ui.item.sortable.model;
                    // restore the removed item
                    ui.item.sortable.sourceModel.push(_.cloneDeep(ui.item.sortable.model));
                    g.page = ui.item.sortable.droptarget.data("page");

                    var modalInstance = this.$uibModal.open({
                        animation: true,
                        templateUrl: 'comp/home/formtemplates/design/forms/_metricGroup/enter.html',
                        controller: 'metricGroupFormController',
                        size: 'lg',
                        resolve: {
                            group: () => {
                                return g;
                            }
                        }
                    });


                }
            }
        };

        metricsDraggableOptions = {
            connectWith: ".metrics",

        };

        metricGroupControl = [];
        freeTextControl = [];
        rateControl = [];
        dateControl = [];
        timeControl = [];
        dichotomousControl = [];
        numericControl = [];
        multipleChoiceControl = [];
        attachmentControl = [];

        static $inject: string[] = ["$scope", "$state", "$stateParams", "$uibModal", "formTemplateResource", "metricGroupResource", "metricResource", "newMetricResource", "$ngBootbox"];

        constructor(
            private $scope: ng.IScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: angular.ui.bootstrap.IModalService,
            private formTemplateResource: Resources.IFormTemplateResource,
            private metricGroupResource: Resources.IMetricGroupResource,
            private metricResource: Resources.IMetricResource,
            private newMetricResource: Resources.INewMetricResource,
            private $ngBootbox: BootboxStatic) {

            this.formId = $stateParams["id"];
            this.survey = <Models.ISurvey>{};
            this.survey.formValues = [];
            this.activate();
        }

        activate() {
            this.formTemplateResource.get({ id: this.formId }).$promise
                .then((formTemplate: Models.IFormTemplate) => {
                    this.formTemplate = formTemplate;
                    var pageGroups = _.groupBy(formTemplate.metricGroups, (mg) => { return mg.page });
                    this.pageNumbers = Object.keys(pageGroups);

                    if (this.pageNumbers.length < 1) {
                        this.addPage();
                    }

                    this.metricGroupResource.get({ 'formTemplateId': this.formTemplate.id, 'id': '00000000-0000-0000-0000-000000000000' }, (group) => {
                        this.metricGroupControl.push(group);

                        if (this.formTemplate.metricGroups.length < 1) {
                            // init with a default metric group.
                            let g: Models.IMetricGroup = _.cloneDeep(group);
                            g.title = "First Metric Group";
                            g.helpContext = "Drag & drop the metrics you want to add to get started!";
                            this.formTemplate.metricGroups.push(g);
                        }
                    });

                    this.newMetricResource.createFreeText((metric) => { this.freeTextControl.push(metric); });
                    this.newMetricResource.createNumeric((metric) => { this.numericControl.push(metric); });
                    this.newMetricResource.createRate((metric) => { this.rateControl.push(metric); });
                    this.newMetricResource.createDate((metric) => { this.dateControl.push(metric); });
                    this.newMetricResource.createTime((metric) => { this.timeControl.push(metric); });
                    this.newMetricResource.createDichotomous((metric) => { this.dichotomousControl.push(metric); });
                    this.newMetricResource.createMultipleChoice((metric) => { this.multipleChoiceControl.push(metric); });
                    this.newMetricResource.createAttachment((metric) => { this.attachmentControl.push(metric); });
                });
        }

        openEditForm() {
            // get calendarDate metrics
            let calendarDateMetrics = [];
            _.forEach(this.formTemplate.metricGroups, (mg) => {
                _.forEach(mg.metrics, (m) => {
                    if (_.toLower(m.type) == "datemetric") {
                        calendarDateMetrics.push({
                            id: m.id,
                            title: m.shortTitle
                        });
                    }
                });
            });

            // get timelineBar metrics
            let timelineBarMetrics = [];
            _.forEach(this.formTemplate.metricGroups, (mg) => {
                _.forEach(mg.metrics, (m) => {
                    if (_.toLower(m.type) == "ratemetric") {
                        timelineBarMetrics.push({
                            id: m.id,
                            title: m.shortTitle
                        });
                    }
                });
            });

            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/formtemplates/design/forms/formTemplate/enter.html',
                controller: 'formTemplateFormController',
                controllerAs: 'ctrl',
                size: 'lg',
                resolve: {
                    formTemplate: () => {
                        return this.formTemplate;
                    },
                    calendarDateMetrics: () => {
                        return calendarDateMetrics;
                    },
                    timelineBarMetrics: () => {
                        return timelineBarMetrics;
                    }
                }
            });
        }

        openEditMetric(metric: Models.IMetric) {
            var m = metric;
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/formtemplates/design/forms/' + _.camelCase(metric.type) + '/enter.html',
                controller: _.camelCase(metric.type) + 'FormController',
                size: 'lg',
                resolve: {
                    metric: () => {
                        return m;
                    }
                }
            });
        }

        openEditGroup(group: Models.IMetricGroup) {
            var g = group;
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/formtemplates/design/forms/_metricGroup/enter.html',
                controller: 'metricGroupFormController',
                size: 'lg',
                resolve: {
                    group: () => {
                        return g;
                    }
                }
            });
        }

        deleteMetric(metric: Models.IMetric) {
            metric.isDeleted = true;
        }

        deleteGroup(group: Models.IMetricGroup) {
            if (_.size(_.filter(group.metrics, { isDeleted: false })) > 0) {
                this.$ngBootbox.alert("Please remove metrics first!");
            }
            else {
                group.isDeleted = true;
            };

        }

        addPage() {
            this.pageNumbers.push((_.size(this.pageNumbers) + 1).toString())
        }

        save() {
            this.formTemplateResource.update(this.formTemplate
                , (obj) => {
                    this.$ngBootbox.alert("Form template saved successfully!");
                }
                , (reason) => {
                    console.log(reason);

                    this.errors = [];
                    for (var key in reason.data.modelState) {
                        for (var i = 0; i < reason.data.modelState[key].length; i++) {
                            this.errors.push(<Error>{ key: key, value: reason.data.modelState[key][i] });
                        }
                    }

                    this.$ngBootbox.alert("Something went wrong! please check errors and try again.");

                });
        }

        addFormValue(metric, rowDataListItem, rowNumber) {
            var formValue = <Models.IFormValue>{};
            formValue.metricId = metric.id;
            formValue.rowNumber = rowNumber;
            if (rowDataListItem)
                formValue.rowDataListItemId = rowDataListItem.id;
            this.survey.formValues.push(formValue);
            return formValue;
        };
    }

    angular.module("app").controller("formDesignController", FormDesignController);
}