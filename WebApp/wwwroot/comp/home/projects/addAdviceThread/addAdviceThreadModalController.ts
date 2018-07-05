module App {
    "use strict";

    interface IAddAdviceThreadModel {
        title: string;
        description: string;
        colour: string;
    }

    interface IAddAdviceThreadModalController {
        model: IAddAdviceThreadModel;

        activate: () => void;
        close: () => void;
        createAdviceThread: (form: ng.IFormController) => void;
    }

    class AddAdviceThreadModalController implements IAddAdviceThreadModalController {
        model: IAddAdviceThreadModel;

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
                title: '',
                description: '',
                colour: ''
            };
        }

        generateColor() {
            this.model.colour = "#" + Math.random().toString(16).slice(2, 8);
        }

        createAdviceThread(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.projectResource.createAdviceThread({ id: this.projectId, title: this.model.title, description: this.model.description, colour: this.model.colour }, (res) => {
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

    angular.module("app").controller("addAdviceThreadModalController", AddAdviceThreadModalController);
}