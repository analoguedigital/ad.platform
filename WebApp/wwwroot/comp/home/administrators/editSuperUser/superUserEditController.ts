module App {
    "use strict";

    interface ISuperUserEditControllerScope extends ng.IScope {
        title: string;
        emailConfirmed: boolean;
        phoneNumberConfirmed: boolean;
    }

    interface ISuperUserEditController {
        user: Models.IUser;
        errors: string;

        submit: (form: ng.IFormController) => void;
        clearErrors: () => void;
        activate: () => void;
    }

    class SuperUserEditController implements ISuperUserEditController {
        user: Models.IUser;
        isInsertMode: boolean;
        errors: string;

        static $inject: string[] = ["$scope", "$state", "$stateParams", "userContextService", "toastr", "userResource"];
        constructor(
            private $scope: ISuperUserEditControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private userContextService: Services.IUserContextService,
            private toastr: any,
            private userResource: Resources.IUserResource
        ) {
            this.$scope.title = "Administrators";
            this.activate();
        }

        activate() {
            var userId = this.$stateParams['id'];
            if (userId === '') {
                this.isInsertMode = true;
                userId = '00000000-0000-0000-0000-000000000000';
            } else {
                this.userResource.get({ id: userId }).$promise.then((user) => {
                    this.user = user;

                    // put these on $scope for lm-form-group bindings
                    this.$scope.emailConfirmed = user.emailConfirmed;
                    this.$scope.phoneNumberConfirmed = user.phoneNumberConfirmed;
                });
            }
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            var userId = this.$stateParams['id'];
            if (userId === '') {
                this.userResource.save(
                    this.user,
                    () => { this.$state.go('home.administrators.list'); },
                    (err) => {
                        if (err.status === 400)
                            this.toastr.error(err.data.message);
                        this.errors = err.data.exceptionMessage;
                    });
            }
            else {
                this.userResource.update(
                    this.user,
                    () => { this.$state.go('home.administrators.list'); },
                    (err) => {
                        if (err.status === 400)
                            this.toastr.error(err.data.message);
                        this.errors = err.data.exceptionMessage;
                    });
            }
        }

        clearErrors() {
            this.errors = undefined;
        }
    }

    angular.module("app").controller("superUserEditController", SuperUserEditController);
}