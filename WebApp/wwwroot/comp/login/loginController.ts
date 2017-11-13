
module App {
    "use strict";

    interface ILoginControllerScope extends ng.IScope {
        title: string;
        loginData: Services.ILoginData;
        login: (form: ng.IFormController) => void;
    }

    interface ILoginController {
        activate: () => void;
        login: (form: ng.IFormController) => void;
    }

    class LoginController implements ILoginController {
        static $inject: string[] = ["$scope", "$state", "userContextService"];

        constructor(
            private $scope: ILoginControllerScope,
            private $state: ng.ui.IStateService,
            private userContextService: App.Services.IUserContextService) {

            this.activate();
        }

        activate() {

            this.$scope.title = "Login";
            this.$scope.loginData = { email: "", password: "" };
            this.$scope.login = (form: ng.IFormController) => { this.login(form); };
        }

        login(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.userContextService.login(this.$scope.loginData).then(
                () => { this.$state.go('home.dashboard.layout'); },
                (err) => { alert(err); });
        };
    }

    angular.module("app").controller("loginController", LoginController);
}