module App {
    "use strict";

    interface ILoginControllerScope extends ng.IScope {
        title: string;
        loginData: Services.ILoginData;
        isWorking: boolean;
        isDownloadRequest: boolean;
        login: (form: ng.IFormController) => void;
    }

    interface ILoginController {
        activate: () => void;
        login: (form: ng.IFormController) => void;
    }

    class LoginController implements ILoginController {
        static $inject: string[] = ["$scope", "$state", "userContextService", "toastr", "RedirectUrlAfterLogin"];
        constructor(
            private $scope: ILoginControllerScope,
            private $state: ng.ui.IStateService,
            private userContextService: App.Services.IUserContextService,
            private toastr: any,
            private RedirectUrlAfterLogin: any) {

            this.activate();
        }

        activate() {
            this.$scope.title = "Login";
            this.$scope.loginData = { email: "", password: "" };
            this.$scope.login = (form: ng.IFormController) => { this.login(form); };

            var redirectUrl = this.RedirectUrlAfterLogin.url;
            if (redirectUrl && redirectUrl.length && redirectUrl !== '/') {
                this.$scope.isDownloadRequest = true;
            }
        }

        login(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.$scope.isWorking = true;
            this.userContextService.login(this.$scope.loginData)
                .then(() => {
                    var redirectUrl = this.RedirectUrlAfterLogin.url;
                    if (redirectUrl.length && redirectUrl !== "/") {
                        var index = redirectUrl.lastIndexOf('/');
                        var downloadId = redirectUrl.substring(index + 1);

                        this.$state.go('downloads', { id: downloadId });
                    } else {
                        this.$state.go('home.dashboard.layout');
                    }
                },
                (err) => {
                    if (err.status && err.status === 401)
                        this.toastr.error(err.data.message);
                    else this.toastr.error(err);
                })
                .finally(() => {
                    this.$scope.isWorking = false;
                });
        };
    }

    angular.module("app").controller("loginController", LoginController);
}