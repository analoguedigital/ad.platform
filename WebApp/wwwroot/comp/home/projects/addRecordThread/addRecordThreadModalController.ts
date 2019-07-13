module App {
    "use strict";

    interface IAddRecordThreadModel {
        code: string;
        title: string;
        description: string;
        colour: string;
    }

    interface IAddRecordThreadModalController {
        model: IAddRecordThreadModel;
        project: Models.IProject;

        activate: () => void;
        close: () => void;
        createRecordThread: (form: ng.IFormController) => void;
    }

    class AddRecordThreadModalController implements IAddRecordThreadModalController {
        model: IAddRecordThreadModel;
        project: Models.IProject;

        static $inject: string[] = ["$uibModalInstance", "projectResource", "toastr", "projectId"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private projectResource: Resources.IProjectResource,
            public toastr: any,
            public projectId: string) {
            this.activate();
        }

        activate() {
            this.model = {
                code: '',
                title: '',
                description: '',
                colour: ''
            };

            this.projectResource.getDirect({ id: this.projectId }).$promise.then((data) => {
                this.project = data;
            });
        }

        generateColor() {
            this.model.colour = "#" + Math.random().toString(16).slice(2, 8);
        }

        createRecordThread(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.projectResource.createRecordThread({ id: this.projectId, code: this.model.code, title: this.model.title, description: this.model.description, colour: this.model.colour }, (res) => {
                this.toastr.success(this.model.title + ' created');
                this.$uibModalInstance.close();
            }, (err) => {
                console.error(err);
                if (err.data.message)
                    this.toastr.error(err.data.message);
            });

            //this.$uibModalInstance.close(this.model);
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("addRecordThreadModalController", AddRecordThreadModalController);
}