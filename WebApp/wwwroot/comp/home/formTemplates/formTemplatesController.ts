
module App {
    "use strict";

    interface Error {
        key: string;
        value: string;
    }

    interface IFormTemplatesControllerScope extends angular.IScope {
        title: string;
        searchTerm: string;

        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        projects: Models.IProject[];
        selectedProject: Models.IProject;

        forms: Models.IFormTemplate[];
        displayedForms: Models.IFormTemplate[];

        adviceThreads: Models.IFormTemplate[];
        displayedAdviceThreads: Models.IFormTemplate[];

        selectedFormTemplate: Models.IFormTemplate;

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
        static $inject: string[] = ["$scope", "$q", "formTemplateResource", "projectResource", "$ngBootbox", "toastr"];

        constructor(
            private $scope: IFormTemplatesControllerScope,
            private $q: ng.IQService,
            private formResource: Resources.IFormTemplateResource,
            private projectResource: Resources.IProjectResource,
            private $ngBootbox: BootboxStatic,
            private toastr: any) {

            $scope.title = "Form Templates";
            $scope.selectedProject = null;

            $scope.delete = (id) => { this.delete(id); };
            $scope.publish = (template) => { this.publish(template); };
            $scope.archive = (template) => { this.archive(template); };

            this.activate();
        }

        activate() {
            var self = this;

            this.$scope.projects = this.projectResource.query();

            this.$scope.customDialogButtons = {
                deleteFormTemplate: {
                    label: "Delete",
                    className: "btn-primary",
                    callback: function (args) {
                        if (self.$scope.selectedFormTemplate) {
                            self.delete(self.$scope.selectedFormTemplate.id);
                        }
                    }
                },
                ForceDeleteFormTemplate: {
                    label: "Force delete",
                    className: "btn-danger",
                    callback: function () {
                        if (self.$scope.selectedFormTemplate) {
                            self.forceDelete(self.$scope.selectedFormTemplate.id);
                        }
                    }
                },
                cancel: {
                    label: "Cancel",
                    className: "btn-default",
                    callback: function () {

                    }
                }
            };

            this.load();
        }

        load() {
            var selectedProject = this.$scope.selectedProject;

            var promise: ng.IPromise<any>;
            var adviceThreadsPromise: ng.IPromise<any>;

            if (selectedProject == null) {
                promise = this.formResource.query({ discriminator: 0 }).$promise;
                adviceThreadsPromise = this.formResource.query({ discriminator: 1 }).$promise;
            }
            else {
                promise = this.formResource.query({ discriminator: 0, projectId: selectedProject.id }).$promise;
                adviceThreadsPromise = this.formResource.query({ discriminator: 1, projectId: selectedProject.id }).$promise;
            }

            this.$q.all([promise, adviceThreadsPromise]).then((data) => {
                this.errors = [];

                var threads = data[0];
                var adviceThreads = data[1];

                this.$scope.forms = threads;
                this.$scope.displayedForms = [].concat(this.$scope.forms);

                this.$scope.adviceThreads = adviceThreads;
                this.$scope.displayedAdviceThreads = [].concat(this.$scope.adviceThreads);
            });

            //promise.then((forms) => {
                
            //});
        }

        selectedFormTemplateChanged(form: Models.IFormTemplate) {
            this.$scope.selectedFormTemplate = form;
        }

        getSharedTemplates() {
            this.$scope.selectedProject = null;
            this.load();
        }

        delete(id: string) {
            this.formResource.delete({ id: id },
                () => { this.load(); },
                (err) => {
                    console.log(err);
                    this.toastr.error(err.data.message);
                });
        }

        forceDelete(id: string) {
            this.formResource.forceDelete({ id: id },
                () => { this.load(); },
                (err) => {
                    console.error(err);
                    this.toastr.error(err.data.message);
                });
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