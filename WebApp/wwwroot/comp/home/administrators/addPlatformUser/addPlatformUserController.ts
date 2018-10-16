module App {
    "use strict";

    interface IAddPlatformUserController {
        user: Models.IUser;
        activate: () => void;
        close: () => void;
    }

    interface IAddPlatformUserModel {
        firstName: string;
        surname: string;
        email: string;
        password: string;
        confirmPassword: string;
    }

    class AddPlatformUserController implements IAddPlatformUserController {
        user: Models.IUser;
        model: IAddPlatformUserModel;
        public selfReset: boolean;
        public isMatch: boolean = true;

        static $inject: string[] = ["$uibModalInstance", "$resource", "userContextService", "platformUserResource", "toastr"];
        constructor(private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private $resource: ng.resource.IResourceService,
            private userContext: Services.IUserContextService,
            private platformUserResource: Resources.IPlatformUserResource,
            private toastr: any) {

            this.activate();
        }

        activate() { }

        checkMatch() {
            if (this.model.password === undefined || this.model.confirmPassword === undefined)
                return true; // no need for the message

            this.isMatch = this.model.password === this.model.confirmPassword;
        }

        createPlatformUser(form: ng.IFormController) {
            if (form.$invalid) {
                this.toastr.warning("Fix your validation errors first");
                return;
            }

            this.platformUserResource.create(this.model, (res) => {
                this.toastr.success('Platform user created successfully');
                this.close();
            }, (err) => {
                console.error(err);
            });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("addPlatformUserController", AddPlatformUserController);
}