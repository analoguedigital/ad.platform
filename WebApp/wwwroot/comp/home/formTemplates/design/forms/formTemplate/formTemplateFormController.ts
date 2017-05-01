
module App {
    "use strict";

    interface IFormTemplateFormController {
        activate: () => void;
        close: () => void;
        formTemplate: Models.IFormTemplate;
        categories: Models.IFormTemplateCategory[];
    }

    class FormTemplateFormController implements IFormTemplateFormController {
        errors: string;
        categories: Models.IFormTemplateCategory[];

        static $inject: string[] = ["$uibModalInstance",  "formTemplateCategoryResource", "formTemplate"];

        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private formTemplateCategoryResource: Resources.IFormTemplateCategoryResource,
            public formTemplate: Models.IFormTemplate
        ) {
            this.formTemplate = formTemplate;
            this.activate();
        }

        activate() {
            this.categories = this.formTemplateCategoryResource.query();
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };

    }

    angular.module("app").controller("formTemplateFormController", FormTemplateFormController);
}