module App {
    "use strict";

    interface IConfirmEmailModel {
        email: string;
    }

    interface IConfirmEmailControllerScope extends ng.IScope {
        model: IConfirmEmailModel;
        sendEmailToken: (form: ng.IFormController) => void;
        requestSent: boolean;
    }

    interface IConfirmEmailController {
        activate: () => void;
        sendEmailToken: (form: ng.IFormController) => void;
    }

    class ConfirmEmailController implements IConfirmEmailController {
        static $inject: string[] = ["$scope", "$resource", "$state", "toastr"];
        constructor(
            private $scope: IConfirmEmailControllerScope,
            private $resource: ng.resource.IResourceService,
            private $state: ng.ui.IStateService,
            private toastr: any) {
            this.activate();
        }

        activate() {
            this.$scope.model = { email: "" };
            this.$scope.sendEmailToken = (form: ng.IFormController) => { this.sendEmailToken(form); }
        }

        sendEmailToken(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.$resource("/api/account/sendEmailConfirmation").save(this.$scope.model).$promise.then(
                (result) => {
                    this.$scope.requestSent = true;
                },
                (err) => {
                    console.error(err);
                    if (err.status === 400) {
                        this.toastr.error(err.data.message);
                    }
                });
        }
    }

    angular.module("app").controller("confirmEmailController", ConfirmEmailController);
}