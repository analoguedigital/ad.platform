
module App {
    "use strict";

    interface ISetPasswordModel {
        email: string;
        code: string;
        password: string;
        confirmPassword: string;
    }

    interface ISetPasswordControllerScope extends ng.IScope {
        model: ISetPasswordModel;
        isMatch: boolean;
        token: string;
        emailAddress: string;

        checkMatch: () => void;
        resetPassword: (form: ng.IFormController) => void;
    }

    interface ISetPasswordController {
        activate: () => void;
        resetPassword: (form: ng.IFormController) => void;
    }

    class SetPasswordController implements ISetPasswordController {
        static $inject: string[] = ["$scope", "$resource", "$state", "$stateParams", "toastr"];
        constructor(
            private $scope: ISetPasswordControllerScope,
            private $resource: ng.resource.IResourceService,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private toastr: any) {
            this.activate();
        }

        activate() {
            this.$scope.model = { email: "", code: "", password: "", confirmPassword: "" };
            this.$scope.isMatch = true;

            this.$scope.checkMatch = () => { this.checkMatch(); }
            this.$scope.resetPassword = (form: ng.IFormController) => { this.resetPassword(form); }

            var token = this.$stateParams.token;
            var email = this.$stateParams.email;

            this.$scope.token = token;
            this.$scope.emailAddress = email;

            if (token && token.length)
                this.$scope.model.code = token;

            if (email && email.length)
                this.$scope.model.email = email;
        }

        checkMatch() {
            this.$scope.isMatch = this.$scope.model.password === this.$scope.model.confirmPassword;
        }

        resetPassword(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            if (!this.$scope.isMatch) {
                return;
            }

            this.$resource("/api/account/resetPassword").save(this.$scope.model).$promise.then(
                (result) => {
                    this.toastr.success("You can now sign in with your new password", "Password Set!");
                    this.$state.go("login");
                },
                (err) => {
                    console.log(err);
                    alert(err.data.message);
                });
        }
    }

    angular.module("app").controller("setPasswordController", SetPasswordController);
}