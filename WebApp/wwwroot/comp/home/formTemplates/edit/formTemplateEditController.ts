
module App {
    "use strict";

    interface IFormTemplateEditController {
        title: string;
        activate: () => void;
        formTemplate: Models.IFormTemplate;
        categories: Models.IFormTemplateCategory[];
        projects: Models.IProject[];
        assignments: Models.IThreadAssignment[];
        errors: string;
        submit: (form: ng.IFormController) => void;
        clearErrors: () => void;
        currentUserIsSuperUser: boolean;
        organisations: Models.IOrganisation[];
    }

    class FormTemplateEditController implements IFormTemplateEditController {
        title: string = "Form template details";
        formTemplateId: string;
        formTemplate: Models.IFormTemplate;
        categories: Models.IFormTemplateCategory[];
        projects: Models.IProject[];
        assignments: Models.IThreadAssignment[];
        errors: string;
        currentUserIsSuperUser: boolean;
        organisations: Models.IOrganisation[];

        static $inject: string[] = ["$scope", "formTemplateResource", "formTemplateCategoryResource",
            "projectResource", "$state", "$stateParams", "$uibModal", "organisationResource", "userContextService", "toastr"];

        constructor(
            private $scope: ng.IScope,
            private formTemplateResource: Resources.IFormTemplateResource,
            private formTemplateCategoryResource: Resources.IFormTemplateCategoryResource,
            private projectResource: Resources.IProjectResource,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private organisationResource: Resources.IOrganisationResource,
            private userContextService: Services.IUserContextService,
            private toastr: any
        ) {
            this.formTemplateId = $stateParams['id'];

            $scope.minicolorSettings = {
                control: 'hue',
                format: 'hex',
                opacity: false,
                theme: 'bootstrap',
                position: 'top left'
            };

            this.activate();
        }

        activate() {
            if (this.formTemplateId === '') {
                this.formTemplateId = '00000000-0000-0000-0000-000000000000';
                this.formTemplateResource.get({ id: this.formTemplateId }).$promise
                    .then((form) => {
                        this.formTemplate = form;
                    });
            } else {
                this.formTemplateResource.get({ id: this.formTemplateId })
                    .$promise.then((form) => {
                        this.formTemplate = form;

                        this.formTemplateResource.getAssignments({ id: this.formTemplateId }, (assignments) => {
                            this.assignments = assignments;
                        });
                    });
            }

            this.categories = this.formTemplateCategoryResource.query();
            this.projects = this.projectResource.query();

            var roles = ["System administrator"];
            this.currentUserIsSuperUser = this.userContextService.userIsInAnyRoles(roles);

            if (this.currentUserIsSuperUser) {
                this.organisationResource.query().$promise.then((organisations) => {
                    this.organisations = organisations;
                });
            }
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                this.toastr.warning("Fix your validation errors first");
                return;
            }

            if (this.formTemplateId === '' || this.formTemplateId === '00000000-0000-0000-0000-000000000000') {
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
            } else {
                this.formTemplateResource.update(
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

        organisationChanged() {
            var org = this.formTemplate.organisation;
            this.projects = this.projectResource.query({ organisationId: org.id });
        }
    }

    angular.module("app").controller("formTemplateEditController", FormTemplateEditController);
}