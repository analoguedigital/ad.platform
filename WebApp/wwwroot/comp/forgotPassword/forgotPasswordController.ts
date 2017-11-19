
module App {
    "use strict";

    interface IForgotPasswordModel {
        email: string;
    }

    interface IForgotPasswordControllerScope extends ng.IScope {
        model: IForgotPasswordModel;
        resetPassword: (form: ng.IFormController) => void;
    }

    interface IForgotPasswordController {
        activate: () => void;
        resetPassword: (form: ng.IFormController) => void;
    }

    class ForgotPasswordController implements IForgotPasswordController {
        static $inject: string[] = ["$scope", "$resource", "$state", "toastr"];
        constructor(
            private $scope: IForgotPasswordControllerScope,
            private $resource: ng.resource.IResourceService,
            private $state: ng.ui.IStateService,
            private toastr: any) {
            this.activate();
        }

        activate() {
            this.$scope.model = { email: "" };
            this.$scope.resetPassword = (form: ng.IFormController) => { this.resetPassword(form); }
        }

        resetPassword(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.$resource("/api/account/forgotPassword").save(this.$scope.model).$promise.then(
                (result) => {
                    this.toastr.info("A password reset token has been sent", "Check your inbox!");
                    this.$state.go("setPassword");
                },
                (err) => {
                    console.log(err);
                    alert(err.data.message);
                });
        }
    }

    angular.module("app").controller("forgotPasswordController", ForgotPasswordController);
}