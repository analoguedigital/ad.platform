declare var Chart: any;

((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes)
        .config(ConfigHttpProvider)
        .config(ConfigDateTransformer)
        .config(ConfigLoadingBar)
        .config(ConfigChartJs);

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
                params: { authenticationRequired: false },
                ncyBreadcrumb: { label: 'Login' },
                data: { bodyCssClass: "login-page" },
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

    ConfigLoadingBar.$inject = ["cfpLoadingBarProvider"];
    function ConfigLoadingBar(cfpLoadingBarProvider) {
        cfpLoadingBarProvider.includeSpinner = false;
    }

    function ConfigChartJs() {
        /* groupableBar implementation */
        /* *************************** */
        Chart.defaults.groupableBar = Chart.helpers.clone(Chart.defaults.bar);

        var helpers = Chart.helpers;
        Chart.controllers.groupableBar = Chart.controllers.bar.extend({
            calculateBarX: function (index, datasetIndex) {
                // position the bars based on the stack index
                var stackIndex = this.getMeta().stackIndex;
                return Chart.controllers.bar.prototype.calculateBarX.apply(this, [index, stackIndex]);
            },

            hideOtherStacks: function (datasetIndex) {
                var meta = this.getMeta();
                var stackIndex = meta.stackIndex;

                this.hiddens = [];
                for (var i = 0; i < datasetIndex; i++) {
                    var dsMeta = this.chart.getDatasetMeta(i);
                    if (dsMeta.stackIndex !== stackIndex) {
                        this.hiddens.push(dsMeta.hidden);
                        dsMeta.hidden = true;
                    }
                }
            },

            unhideOtherStacks: function (datasetIndex) {
                var meta = this.getMeta();
                var stackIndex = meta.stackIndex;

                for (var i = 0; i < datasetIndex; i++) {
                    var dsMeta = this.chart.getDatasetMeta(i);
                    if (dsMeta.stackIndex !== stackIndex) {
                        dsMeta.hidden = this.hiddens.unshift();
                    }
                }
            },

            calculateBarY: function (index, datasetIndex) {
                this.hideOtherStacks(datasetIndex);
                var barY = Chart.controllers.bar.prototype.calculateBarY.apply(this, [index, datasetIndex]);
                this.unhideOtherStacks(datasetIndex);
                return barY;
            },

            calculateBarBase: function (datasetIndex, index) {
                this.hideOtherStacks(datasetIndex);
                var barBase = Chart.controllers.bar.prototype.calculateBarBase.apply(this, [datasetIndex, index]);
                this.unhideOtherStacks(datasetIndex);
                return barBase;
            },

            getBarCount: function () {
                var stacks = [];

                // put the stack index in the dataset meta
                Chart.helpers.each(this.chart.data.datasets, function (dataset, datasetIndex) {
                    var meta = this.chart.getDatasetMeta(datasetIndex);
                    if (meta.bar && this.chart.isDatasetVisible(datasetIndex)) {
                        var stackIndex = stacks.indexOf(dataset.stack);
                        if (stackIndex === -1) {
                            stackIndex = stacks.length;
                            stacks.push(dataset.stack);
                        }
                        meta.stackIndex = stackIndex;
                    }
                }, this);

                this.getMeta().stacks = stacks;
                return stacks.length;
            },
        });
    }
})();