declare var Chart: any;

((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes)
        .config(ConfigHttpProvider)
        .config(ConfigDateTransformer)
        .config(ConfigLoadingBar)
        .config(ConfigUITour);

    angular.module("app")
        .value('RedirectUrlAfterLogin', { url: '/' });

    ConfigUITour.$inject = ["TourConfigProvider"];
    function ConfigUITour(TourConfigProvider) {
        TourConfigProvider.enableNavigationInterceptors();
    }

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("index", {
                abstract: true,
                template: "<ui-view />",
                controller: "indexController"
            })
            .state("home", <App.Models.IAppRoute>{
                abstract: true,
                url: "",
                templateUrl: "comp/home/homeView.html",
                controller: "homeController",
                params: { authenticationRequired: true },
                resolve: {
                    userContext: ['userContextService', (userContextService) => { return userContextService.initialize(); }]
                },
                module: "public"
            })
            .state("login", <App.Models.IAppRoute>{
                parent: "index",
                url: "/login",
                templateUrl: "comp/login/loginView.html",
                controller: "loginController",
                controllerAs: "ctrl",
                params: { authenticationRequired: false },
                ncyBreadcrumb: { label: 'Login' },
                data: { bodyCssClass: "login-page" },
                module: "public"
            })
            .state("forgotPassword", <App.Models.IAppRoute>{
                parent: "index",
                url: "/forgot-password",
                templateUrl: "comp/forgotPassword/forgotPasswordView.html",
                controller: "forgotPasswordController",
                ncyBreadcrumb: { label: 'Forgot Password' },
                data: { bodyCssClass: "forgot-password-page" },
                module: "public"
            })
            .state("setPassword", <App.Models.IAppRoute>{
                parent: "index",
                url: "/set-password?email&token",
                templateUrl: "comp/setPassword/setPasswordView.html",
                controller: "setPasswordController",
                ncyBreadcrumb: { label: 'Reset Password' },
                data: { bodyCssClass: "set-password-page" },
                module: "public"
            })
            .state("confirmEmail", <App.Models.IAppRoute>{
                parent: "index",
                url: "/confirm-email",
                templateUrl: "comp/emailConfirmation/sendToken/sendEmailTokenView.html",
                controller: "confirmEmailController",
                ncyBreadcrumb: { label: 'Confirm Email Address' },
                data: { bodyCssClass: "confirm-email-page" },
                module: "public"
            })
            .state("verifyEmail", <App.Models.IAppRoute>{
                parent: "index",
                url: "/verify-email?userId&code",
                templateUrl: "comp/emailConfirmation/verifyToken/verifyEmailTokenView.html",
                controller: "verifyEmailController",
                ncyBreadcrumb: { label: 'Verify Email Address' },
                data: { bodyCssClass: 'verify-email-page' },
                module: "public"
            });


        $urlRouterProvider.otherwise('/');
    }

    ConfigHttpProvider.$inject = ["$httpProvider"];
    function ConfigHttpProvider($httpProvider: ng.IHttpProvider) {
        $httpProvider.interceptors.push('authInterceptorService');
    }

    ConfigDateTransformer.$inject = ["$httpProvider"];
    function ConfigDateTransformer($httpProvider: ng.IHttpProvider) {
        (<ng.IHttpResponseTransformer[]>$httpProvider.defaults.transformResponse).push(function (responseData) {
            convertDateStringsToDates(responseData);
            return responseData;
        });
    }

    /* tslint:disable */
    var regexIso8601 = /^(\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.\d +)|(\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d)|(\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d)$/;
    /* tslint:enable */

    function convertDateStringsToDates(input) {
        // Ignore things that aren't objects.
        if (typeof input !== "object") { return input; }

        for (var key in input) {
            if (!input.hasOwnProperty(key)) { continue; }

            var value = input[key];
            var match;
            // Check for string properties which look like dates.
            if (typeof value === "string" && (match = value.match(regexIso8601))) {
                var dateValue = match[0];   // ISO-8601 string

                var utcDate = moment.utc(dateValue);
                var hours = utcDate.hour();
                var minutes = utcDate.minutes();
                var localDate = utcDate.local().toDate();

                if (hours === 0 && minutes === 0) {
                    localDate.setHours(0);
                    localDate.setMinutes(0);
                }

                input[key] = localDate;
            } else if (typeof value === "object") {
                // Recurse into object
                convertDateStringsToDates(value);
            }
        }
    }

    ConfigLoadingBar.$inject = ["cfpLoadingBarProvider"];
    function ConfigLoadingBar(cfpLoadingBarProvider) {
        cfpLoadingBarProvider.includeSpinner = false;
    }

})();