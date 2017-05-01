﻿
module App {
    "use strict";

    interface IFormTemplatesControllerScope extends angular.IScope {
        title: string;
        searchTerm: string;
        forms: Models.IFormTemplate[];
        projects: Models.IProject[];
        displayedForms: Models.IFormTemplate[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
        selectedProjectId: string;
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
        static $inject: string[] = ["$scope", "formTemplateResource", "projectResource"];

        constructor(
            private $scope: IFormTemplatesControllerScope,
            private formResource: Resources.IFormTemplateResource,
            private projectResource: Resources.IProjectResource) {

            $scope.title = "Form Templates";
            $scope.selectedProjectId = null;
            this.$scope.$watch('selectedProjectId', () => { this.load(); });
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
            this.formResource.query().$promise.then((forms) => {
                this.$scope.forms = _.filter(forms, { 'projectId': this.$scope.selectedProjectId });
                this.$scope.displayedForms = [].concat(this.$scope.forms);
            });
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
                (err) => { console.log(err); });
        }
    }

    angular.module("app").controller("formTemplatesController", FormTemplatesController);
}