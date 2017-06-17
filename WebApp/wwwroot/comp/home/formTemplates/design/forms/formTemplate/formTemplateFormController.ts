
module App {
    "use strict";

    interface IFormTemplateFormController {
        activate: () => void;
        close: () => void;
        formTemplate: Models.IFormTemplate;
        categories: Models.IFormTemplateCategory[];
        tags: string[];
        selectedTags: string[];
    }

    class FormTemplateFormController implements IFormTemplateFormController {
        errors: string;
        categories: Models.IFormTemplateCategory[];
        tags: string[] = [];
        selectedTags: string[] = [];
        descriptionFormatRegex: RegExp = /{{([^}]+)}}/g;
        descriptionFormatValueRegex: RegExp = /{{(.*?)}}/;

        static $inject: string[] = ["$scope", "$q", "$uibModalInstance", "formTemplateCategoryResource", "formTemplate", "metricOptions"];

        constructor(
            private $scope: ng.IScope,
            private $q: ng.IQService,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private formTemplateCategoryResource: Resources.IFormTemplateCategoryResource,
            public formTemplate: Models.IFormTemplate,
            public metricOptions: any[]
        ) {
            this.formTemplate = formTemplate;
            this.activate();
        }

        activate() {
            this.$scope.minicolorSettings = {
                control: 'hue',
                format: 'hex',
                opacity: false,
                theme: 'bootstrap',
                position: 'top left'
            };

            this.categories = this.formTemplateCategoryResource.query();

            // populate tags options
            _.forEach(this.formTemplate.metricGroups, (mg) => {
                _.forEach(mg.metrics, (m) => {
                    if (_.toLower(m.type) !== App.Models.MetricTypes.AttachmentMetric)
                        this.tags.push(m.shortTitle);
                });
            });

            // read description format
            let format = this.formTemplate.descriptionFormat;
            if (format && format.length) {
                var props = format.match(this.descriptionFormatRegex);
                let result = [];
                _.forEach(props, (p) => {
                    let value = p.match(this.descriptionFormatValueRegex)[1];
                    result.push(value);
                });

                this.selectedTags = result;
            }
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };

        loadTags(query: string) {
            let deferred = this.$q.defer();

            let result = _.filter(this.tags, (tag) => { return _.startsWith(_.toLower(tag), _.toLower(query)); });
            deferred.resolve(result);

            return deferred.promise;
        }

        getDescriptionFormat() {
            let labels = [];
            _.forEach(this.selectedTags, (t: any) => {
                labels.push(`{{${t.text}}}`);
            });

            let result = _.toLower(labels.join(' - '));
            return result;
        }

        updateDescriptionFormat() {
            this.formTemplate.descriptionFormat = this.getDescriptionFormat();
        }

    }

    angular.module("app").controller("formTemplateFormController", FormTemplateFormController);
}