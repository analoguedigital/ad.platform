
module App {
    "use strict";

    interface Error {
        key: string;
        value: string;
    }

    interface IFormTemplatesControllerScope extends angular.IScope {
        title: string;
        searchTerm: string;
        forms: Models.IFormTemplate[];
        projects: Models.IProject[];
        displayedForms: Models.IFormTemplate[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
        selectedProject: Models.IProject;
        delete: (id: string) => void;
        publish: (template: Models.IFormTemplate) => void;
        archive: (template: Models.IFormTemplate) => void;
    }

    interface IFormTemplatesController {
        activate: () => void;
        delete: (id: string) => void;
        publish: (template: Models.IFormTemplate) => void;
        archive: (template: Models.IFormTemplate) => void;
    }

    class FormTemplatesController implements IFormTemplatesController {
        errors: Error[] = [];
        static $inject: string[] = ["$scope", "formTemplateResource", "projectResource", "$ngBootbox"];

        constructor(
            private $scope: IFormTemplatesControllerScope,
            private formResource: Resources.IFormTemplateResource,
            private projectResource: Resources.IProjectResource,
            private $ngBootbox: BootboxStatic) {

            $scope.title = "Form Templates";
            $scope.selectedProject = null;
            $scope.delete = (id) => { this.delete(id); };
            $scope.publish = (template) => { this.publish(template); };
            $scope.archive = (template) => { this.archive(template); };
            this.activate();
        }

        activate() {
            this.$scope.projects = this.projectResource.query();
            this.load();
        }

        load() {
            var selectedProject = this.$scope.selectedProject;

            var promise: ng.IPromise<any>;
            if (selectedProject == null)
                promise = this.formResource.query().$promise;
            else
                promise = this.formResource.query({ projectId: selectedProject.id }).$promise;

            promise.then((forms) => {
                this.$scope.forms = forms;
                this.$scope.displayedForms = [].concat(this.$scope.forms);
                this.errors = [];
            });
        }

        getSharedTemplates() {
            this.$scope.selectedProject = null;
            this.load();    
        }

        delete(id: string) {
            this.formResource.delete({ id: id },
                () => { this.load(); },
                (err) => { console.log(err); });
        }

        archive(template: Models.IFormTemplate) {
            this.formResource.archive({ id: template.id },
                () => { this.load(); },
                (err) => { console.log(err); });
        }

        publish(template: Models.IFormTemplate) {
            this.formResource.publish({ id: template.id },
                () => { this.load(); },
                (reason) => {
                    this.errors = [];
                    for (var key in reason.data.modelState) {
                        for (var i = 0; i < reason.data.modelState[key].length; i++) {
                            this.errors.push(<Error>{ key: key, value: reason.data.modelState[key][i] });
                        }
                    }

                    console.log(this.errors);

                    this.$ngBootbox.alert("Something went wrong! please check errors and try again.");
                });
        }

        clearErrors() {
            this.errors = [];
        }
    }

    angular.module("app").controller("formTemplatesController", FormTemplatesController);
}