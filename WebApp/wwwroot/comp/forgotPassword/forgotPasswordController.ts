module App {
    "use strict";

    interface IForgotPasswordModel {
        email: string;
    }

    interface IForgotPasswordControllerScope extends ng.IScope {
        model: IForgotPasswordModel;
        resetPassword: (form: ng.IFormController) => void;
        requestSent: boolean;
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
                    this.$scope.requestSent = true;
                },
                (err) => {
                    console.error(err);
                    if (err.status === 400) {
                        this.toastr.error('Seriously? For real?', 'Bad Request');
                    }
                });
        }
    }

    angular.module("app").controller("forgotPasswordController", ForgotPasswordController);
}