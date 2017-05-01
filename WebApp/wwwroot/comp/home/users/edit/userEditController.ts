
module App {
    "use strict";

    interface IUserEditController {
        title: string;
        user: Models.IOrgUser;
        types: Models.IOrgUserType[];
        errors: string;
        submit: (form: ng.IFormController) => void;
        clearErrors: () => void;

        activate: () => void;
    }

    class UserEditController implements IUserEditController {

        title: string;
        user: Models.IOrgUser;
        types: Models.IOrgUserType[];
        isInsertMode: boolean;
        errors: string;


        static $inject: string[] = [ "orgUserResource", "orgUserTypeResource", "$state", "$stateParams"];

        constructor(
            private orgUserResource: Resources.IOrgUserResource,
            private orgUserTypeResource: Resources.IOrgUserTypeResource,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService
        ) {

            this.title = "Users";
            
            this.activate();
        }

        activate() {
            var userId = this.$stateParams['id'];
            if (userId === '') {
                this.isInsertMode = true;
                userId = '00000000-0000-0000-0000-000000000000';
            }
            this.orgUserTypeResource.query().$promise.then((types) => {
                this.types = types;
            });

            this.orgUserResource.get({ id: userId }).$promise.then((user) => {
                this.user = user;
            });
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }
            var userId = this.$stateParams['id'];
            if (userId === '') {
                this.orgUserResource.save(
                    this.user,
                    () => { this.$state.go('home.users.list'); },
                    (err) => {
                        console.log(err);
                        this.errors = err.data.exceptionMessage;
                    });
            }
            else {
                this.orgUserResource.update(
                    this.user,
                    () => { this.$state.go('home.users.list'); },
                    (err) => {
                        console.log(err);
                        this.errors = err.data.exceptionMessage;
                    });
            }
        }

        clearErrors() {
            this.errors = undefined;
        }

    }

    angular.module("app").controller("userEditController", UserEditController);
}