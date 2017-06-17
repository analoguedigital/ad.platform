
((): void => {
    "use strict";

    angular.module("app")
        .run(addRouteStateInfoToRootScope)
        .run(configAuthenticationCheck)
        .run(initializeUserContextService)
        .run(configAngularMoment);

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

    configAuthenticationCheck.$inject = ["$rootScope", "$state", "authService"];
    function configAuthenticationCheck(
        $rootScope: ng.IRootScopeService,
        $state: ng.ui.IStateService,
        authService: App.Services.IAuthService) {

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
            });
    }

    initializeUserContextService.$inject = ["userContextService"];
    function initializeUserContextService(userContextService: App.Services.IUserContextService) {
        userContextService.initialize();
    }

    configAngularMoment.$inject = ["amMoment", "$locale"];
    function configAngularMoment(amMoment, $locale) {
        amMoment.changeLocale($locale.id);
    }

})();