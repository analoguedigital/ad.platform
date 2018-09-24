((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.emailRecipients", <App.Models.IAppRoute>{
                url: "/email-recipients",
                templateUrl: "comp/home/emailRecipients/emailRecipientsView.html",
                controller: "emailRecipientsController",
                controllerAs: 'ctrl',
                ncyBreadcrumb: { label: 'Email Recipients' },
                module: "private"
            });
    }
})();