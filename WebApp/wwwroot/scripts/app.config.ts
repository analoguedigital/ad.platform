
((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes)
        .config(ConfigHttpProvider)
        .config(ConfigDateTransformer);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("index", {
                abstract: true,
                template: "<ui-view />",
                controller: "indexController"
            })
            .state("home", {
                abstract: true,
                url: "",
                templateUrl: "comp/home/homeView.html",
                controller: "homeController",
                params: { authenticationRequired: true },
                resolve: {
                    userContext: ['userContextService', (userContextService) => { return userContextService.initialize(); }]
                }
            })
            .state("login", {
                parent: "index",
                url: "/login",
                templateUrl: "comp/login/loginView.html",
                controller: "loginController",
                params: { authenticationRequired: false },
                ncyBreadcrumb: { label: 'Login' },
                data: { bodyCssClass: "login-page" }

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
                // Assume that Date.parse can parse ISO 8601 strings, or has been shimmed in older browsers to do so.
                var milliseconds = Date.parse(match[0]);
                if (!isNaN(milliseconds)) {
                    input[key] = new Date(milliseconds);
                }
            } else if (typeof value === "object") {
                // Recurse into object
                convertDateStringsToDates(value);
            }
        }
    }
})();