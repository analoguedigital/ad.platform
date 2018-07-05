
module App {
    "use strict";

    interface Error {
        key: string;
        value: string;
    }

    interface IThreadAssignmentsControllerScope extends angular.IScope {
        title: string;
        searchTerm: string;
        forms: Models.IFormTemplate[];
        projects: Models.IProject[];
        displayedForms: Models.IFormTemplate[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
        selectedProject: Models.IProject;
    }

    interface IThreadAssignmentsController {
        activate: () => void;
    }

    class ThreadAssignmentsController implements IThreadAssignmentsController {
        errors: Error[] = [];
        static $inject: string[] = ["$scope", "formTemplateResource", "projectResource", "$ngBootbox", "toastr"];

        constructor(
            private $scope: IThreadAssignmentsControllerScope,
            private formResource: Resources.IFormTemplateResource,
            private projectResource: Resources.IProjectResource,
            private $ngBootbox: BootboxStatic,
            private toastr: any) {

            $scope.title = "Thread Assignments";
            $scope.selectedProject = null;

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
                promise = this.formResource.query({ discriminator: 0 }).$promise;
            else
                promise = this.formResource.query({ discriminator: 0, projectId: selectedProject.id }).$promise;

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
    }

    angular.module("app").controller("threadAssignmentsController", ThreadAssignmentsController);
}