// Install the angularjs.TypeScript.DefinitelyTyped NuGet package
module App {
    "use strict";

    interface IAddFormTemplateCategoryController {
        category: Models.IFormTemplateCategory;
        activate: () => void;
    }

    class AddFormTemplateCategoryController implements IAddFormTemplateCategoryController {

        category: Models.IFormTemplateCategory;

        static $inject: string[] = ["$uibModalInstance", "formTemplateCategoryResource"];

        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private formTemplateCategoryResource: Resources.IFormTemplateCategoryResource) {

            this.activate();
        }

        activate() { }

        save(form: ng.IFormController) {

            if (form.$invalid)
                return;

            this.formTemplateCategoryResource.save(this.category,
                (data) => {
                    this.$uibModalInstance.close(data);
                },
                (err) => { });
        }

        close() {
            this.$uibModalInstance.dismiss();
        }

    }

    angular.module("app").controller("addFormTemplateCategoryController", AddFormTemplateCategoryController);
}