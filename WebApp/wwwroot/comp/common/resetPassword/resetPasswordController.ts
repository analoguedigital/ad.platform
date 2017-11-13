
module App {
    "use strict";

    interface IResetPasswordController {
        user: Models.IUser;
        activate: () => void;
        close: () => void;
    }

    interface IResetPasswordModel {
        userId: string;
        oldPassword: string;
        newPassword: string;
        newConfirmPassword: string;
    }

    class ResetPasswordController implements IResetPasswordController {

        model: IResetPasswordModel;
        public selfReset: boolean;
        public isMatch: boolean = true;

        static $inject: string[] = ["$uibModalInstance", "$resource", "userContextService", "user"];
        constructor(private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private $resource: ng.resource.IResourceService,
            private userContext: Services.IUserContextService,
            public user: Models.IUser) {

            if (user === null) {
                this.selfReset = true;
                this.user = userContext.current.user;
            }
            this.activate();
        }

        activate() {

        }

        checkMatch() {
            if (this.model.newPassword === undefined || this.model.newConfirmPassword === undefined)
                return true; // no need for the message

            this.isMatch = this.model.newPassword === this.model.newConfirmPassword;
        }

        resetPassword(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.model.userId = this.user.id;
            this.$resource("/api/changePassword").save(this.model).$promise.then(
                () => { this.$uibModalInstance.close(); },
                (err) => { console.log(err); alert(err.data.message); });

        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("resetPasswordController", ResetPasswordController);
}