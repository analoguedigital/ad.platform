
module App {
    "use strict";

    interface IVerifyEmailModel {
        userId: string;
        code: string;
    }

    interface IVerifyEmailControllerScope extends ng.IScope {
        model: IVerifyEmailModel;
        requestSent: boolean;
    }

    interface IVerifyEmailController {
        activate: () => void;
    }

    class VerifyEmailController implements IVerifyEmailController {
        static $inject: string[] = ["$scope", "$resource", "$state", "$stateParams", "toastr"];
        constructor(
            private $scope: IVerifyEmailControllerScope,
            private $resource: ng.resource.IResourceService,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private toastr: any) {
            this.activate();
        }

        activate() {
            this.$scope.model = <IVerifyEmailModel>{
                userId: this.$stateParams.userId,
                code: this.$stateParams.code
            };

            console.info(this.$scope.model);

            if (!this.$scope.model.userId || !this.$scope.model.code) 
                this.$state.go('login')

            this.$resource("/api/account/confirmEmail").save(this.$scope.model).$promise.then(
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

    angular.module("app").controller("verifyEmailController", VerifyEmailController);
}