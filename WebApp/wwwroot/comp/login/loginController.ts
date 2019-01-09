module App {
    "use strict";

    interface IVerificationData {
        token: string;
        userName: string;
        password: string;
    }

    interface ILoginControllerScope extends ng.IScope {
        title: string;
        loginData: Services.ILoginData;
        verificationData: IVerificationData;

        isDownloadRequest: boolean;
        requiresVerification: boolean;

        login: (form: ng.IFormController) => void;
        verifyCode: (form: ng.IFormController) => void;

        isWorking: boolean;
        isVerifying: boolean;
    }

    interface ILoginController {
        activate: () => void;
        login: (form: ng.IFormController) => void;
    }

    class LoginController implements ILoginController {
        static $inject: string[] = ["$scope", "$state", "userContextService", "authService", "$timeout", "toastr", "$resource", "RedirectUrlAfterLogin"];
        constructor(
            private $scope: ILoginControllerScope,
            private $state: ng.ui.IStateService,
            private userContextService: App.Services.IUserContextService,
            private authService: App.Services.IAuthService,
            private $timeout: ng.ITimeoutService,
            private toastr: any,
            private $resource: ng.resource.IResourceService,
            private RedirectUrlAfterLogin: any) {

            this.activate();
        }

        activate() {
            this.$scope.title = "Login";
            this.$scope.loginData = { email: "", password: "" };
            this.$scope.verificationData = { token: '', userName: '', password: '' };

            this.$scope.login = (form: ng.IFormController) => { this.login(form); };
            this.$scope.verifyCode = (form: ng.IFormController) => { this.verifyCode(form); };

            this.$scope.requiresVerification = false;

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
                    if (err.error && err.error === 'requires_verification') {
                        this.$scope.requiresVerification = true;
                        return;
                    }

                    if (err.status && err.status === 401)
                        this.toastr.error(err.data.message);
                    else {
                        this.toastr.error(err);
                    }
                })
                .finally(() => {
                    this.$scope.isWorking = false;
                });
        };

        verifyCode(form: ng.IFormController) {
            if (form.$invalid) {
                this.toastr.error('Enter security code first');
                return;
            }

            this.$scope.verificationData.userName = this.$scope.loginData.email;
            this.$scope.verificationData.password = this.$scope.loginData.password;

            this.$scope.isVerifying = true;
            this.$resource("/api/account/verifySecurityCode").save(this.$scope.verificationData).$promise.then(
                (result) => {
                    this.userContextService.twoFactorLogin(this.$scope.loginData, this.$scope.verificationData.token)
                        .then((res) => {
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
                            else {
                                this.toastr.error(err);
                            }
                        });
                },
                (err) => {
                    console.error(err);
                    this.toastr.error(err.data.message);
                })
                .finally(() => {
                    this.$scope.isVerifying = false;
                });
        };

    }

    angular.module("app").controller("loginController", LoginController);
}