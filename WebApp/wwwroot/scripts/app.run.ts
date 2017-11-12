
((): void => {
    "use strict";

    angular.module("app")
        .run(addRouteStateInfoToRootScope)
        .run(initializeUserContextService)
        .run(configAngularMoment)
        .run(configAuthenticationCheck)
        .run(getAuthDataFromUrl);


    getAuthDataFromUrl.$inject = ["authService"]
    function getAuthDataFromUrl(
        authService: App.Services.IAuthService) {

        let authdata = getParameterByName("authData", null);
        if (authdata) {
            authService.loginUser(JSON.parse(authdata));
        }
    }

    function getParameterByName(name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    }

    addRouteStateInfoToRootScope.$inject = ["$rootScope", "$state", "$stateParams"];
    function addRouteStateInfoToRootScope(
        $rootScope: App.IAppRootScopeService,
        $state: ng.ui.IStateService,
        $stateParams: ng.ui.IStateParamsService) {

        // It's very handy to add references to $state and $stateParams to the $rootScope
        // so that you can access them from any scope within your applications.For example,
        // <li ng-class="{ active: $state.includes('contacts.list') }"> will set the <li>
        // to active whenever 'contacts.list' or one of its decendents is active.
        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;
    }

    configAuthenticationCheck.$inject = ["$rootScope", "$state", "authService", "userContextService"];
    function configAuthenticationCheck(
        $rootScope: ng.IRootScopeService,
        $state: ng.ui.IStateService,
        authService: App.Services.IAuthService,
        userContextService: App.Services.UserContextService) {

        $rootScope.$on("$stateChangeStart",
            function (
                event: ng.IAngularEvent,
                toState: ng.ui.IState,
                toParams: ng.ui.IStateParamsService,
                fromState: ng.ui.IState,
                fromParams: ng.ui.IStateParamsService) {

                if (toParams["authenticationRequired"] && !authService.authContext.isAuth) {
                    $state.transitionTo("login");
                    event.preventDefault();
                }

                let _toState = <App.Models.IAppRoute>toState;
                if (_toState.module == "private") {
                    if (userContextService.current.user && userContextService.userIsRestricted()) {
                        event.preventDefault();
                        $state.transitionTo("home.subscriptions.list");
                    }
                }
            });
    }

    initializeUserContextService.$inject = ["userContextService"];
    function initializeUserContextService(userContextService: App.Services.IUserContextService) {
        userContextService.initialize();
    }

    configAngularMoment.$inject = ["amMoment", "$locale", "moment", "$rootScope"];
    function configAngularMoment(amMoment, $locale, moment, $rootScope) {
        var locale = window.navigator.userLanguage || window.navigator.language;
        var localeData = moment().locale(locale).localeData();
        var format = localeData.longDateFormat('L');

        // lower year/day segments for bootstrap datetimepicker to work.
        var inputFormat = _.replace(format, 'DD', 'dd');
        inputFormat = _.replace(inputFormat, 'YYYY', 'yyyy');

        $rootScope.INPUT_DATE_FORMAT = inputFormat;

        amMoment.changeLocale(locale);
    }

})();