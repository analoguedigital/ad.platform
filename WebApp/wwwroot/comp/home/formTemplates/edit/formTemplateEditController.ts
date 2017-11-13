
module App {
    "use strict";

    interface IFormTemplateEditController {
        title: string;
        activate: () => void;
        formTemplate: Models.IFormTemplate;
        categories: Models.IFormTemplateCategory[];
        projects: Models.IProject[];
        errors: string;
        submit: (form: ng.IFormController) => void;
        clearErrors: () => void;
    }

    class FormTemplateEditController implements IFormTemplateEditController {
        title: string = "Form template details";
        formTemplateId: string;
        formTemplate: Models.IFormTemplate;
        categories: Models.IFormTemplateCategory[];
        projects: Models.IProject[];
        errors: string;

        static $inject: string[] = ["$scope", "formTemplateResource", "formTemplateCategoryResource", "projectResource", "$state", "$stateParams", "$uibModal"];

        constructor(
            private $scope: ng.IScope,
            private formTemplateResource: Resources.IFormTemplateResource,
            private formTemplateCategoryResource: Resources.IFormTemplateCategoryResource,
            private projectResource: Resources.IProjectResource,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService
        ) {
            this.formTemplateId = $stateParams['id'];
            this.activate();
        }

        activate() {
            if (this.formTemplateId === '')
                this.formTemplateId = '00000000-0000-0000-0000-000000000000';

            this.formTemplateResource.get({ id: this.formTemplateId })
                .$promise.then((form) => {
                    this.formTemplate = form;
                });

            this.categories = this.formTemplateCategoryResource.query();
            this.projects = this.projectResource.query();
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }
            this.formTemplateResource.save(
                this.formTemplate,
                () => { this.$state.go('home.formtemplates.list'); },
                (err) => {
                    console.log(err);
                    this.errors = err.data.message;
                    if (err.data.exceptionMessage)
                        this.errors += err.data.exceptionMessage;

                    var innerException = err.data.innerException;
                    while (innerException) {
                        this.errors += innerException.exceptionMessage;
                        innerException = innerException.innerException;
                    }
                });
        }

        clearErrors() {
            this.errors = undefined;
        }

        addCategory() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/common/addFormTemplateCategory/addFormTemplateCategoryView.html',
                controller: 'addFormTemplateCategoryController',
                controllerAs: 'ctrl',
                size: 'sm',
            });

            modalInstance.result.then((newCategory: Models.IFormTemplateCategory) => {
                this.categories.push(newCategory);
                this.formTemplate.formTemplateCategory = newCategory;
            });
        }
    }

    angular.module("app").controller("formTemplateEditController", FormTemplateEditController);
}