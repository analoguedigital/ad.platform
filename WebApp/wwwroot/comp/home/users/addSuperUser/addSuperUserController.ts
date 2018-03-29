module App {
    "use strict";

    interface IAddSuperUserController {
        user: Models.IUser;
        activate: () => void;
        close: () => void;
    }

    interface IAddSuperUserModel {
        email: string;
        password: string;
        confirmPassword: string;
    }

    class AddSuperUserController implements IAddSuperUserController {
        user: Models.IUser;
        model: IAddSuperUserModel;
        public selfReset: boolean;
        public isMatch: boolean = true;

        static $inject: string[] = ["$uibModalInstance", "$resource", "userContextService", "userResource", "toastr"];
        constructor(private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private $resource: ng.resource.IResourceService,
            private userContext: Services.IUserContextService,
            private userResource: Resources.IUserResource,
            private toastr: any) {

            this.activate();
        }

        activate() { }

        checkMatch() {
            if (this.model.password === undefined || this.model.confirmPassword === undefined)
                return true; // no need for the message

            this.isMatch = this.model.password === this.model.confirmPassword;
        }

        createSuperUser(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.userResource.createSuperUser(this.model, (res) => {
                this.toastr.success('Super user created successfully');
                this.close();
            }, (err) => {
                console.error(err);
            });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("addSuperUserController", AddSuperUserController);
}