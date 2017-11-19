
module App {
    "use strict";

    interface IFormTemplateFormController {
        activate: () => void;
        close: () => void;
        formTemplate: Models.IFormTemplate;
        categories: Models.IFormTemplateCategory[];
        tags: string[];
    }

    class FormTemplateFormController implements IFormTemplateFormController {
        errors: string;
        categories: Models.IFormTemplateCategory[];
        tags: string[] = [];
        descriptionFormatRegex: RegExp = /{{([^}]+)}}/g;
        descriptionFormatValueRegex: RegExp = /{{(.*?)}}/;

        static $inject: string[] = ["$scope", "$q", "$uibModalInstance", "formTemplateCategoryResource", "formTemplate", "calendarDateMetrics", "timelineBarMetrics"];

        constructor(
            private $scope: ng.IScope,
            private $q: ng.IQService,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private formTemplateCategoryResource: Resources.IFormTemplateCategoryResource,
            public formTemplate: Models.IFormTemplate,
            public calendarDateMetrics: any[],
            public timelineBarMetrics: any[]
        ) {
            this.formTemplate = formTemplate;
            this.activate();
        }

        activate() {
            this.categories = this.formTemplateCategoryResource.query();

            this.$scope.minicolorSettings = {
                control: 'hue',
                format: 'hex',
                opacity: false,
                theme: 'bootstrap',
                position: 'top left'
            };

            // populate tags options
            _.forEach(this.formTemplate.metricGroups, (mg) => {
                _.forEach(mg.metrics, (m) => {
                    if (_.toLower(m.type) !== App.Models.MetricTypes.AttachmentMetric)
                        this.tags.push(_.toLower(m.shortTitle));
                });
            });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };

    }

    angular.module("app").controller("formTemplateFormController", FormTemplateFormController);
}