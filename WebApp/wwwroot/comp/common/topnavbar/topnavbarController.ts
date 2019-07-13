
module App {
    "use strict";

    interface ITopnavbarControllerScope extends ng.IScope {
        title: string;
        user: string;
        $state: ng.ui.IStateService;

        //colorMode: string;

        logout: () => void;
    }

    interface ITopnavbarController {
        activate: () => void;
        logout: () => void;
    }

    class TopnavbarController implements ITopnavbarController {
        static $inject = ["$scope", "userContextService"];
        constructor(
            private $scope: ITopnavbarControllerScope,
            private userContext: Services.IUserContextService) {

            $scope.title = "top navbar";
            $scope.user = userContext.current.user.email;
            //$scope.colorMode = 'light';

            $scope.logout = () => { this.logout(); };
            //$scope.switchColorMode = () => { this.switchColorMode(); };

            this.activate();
        }

        activate() { }

        logout() {
            this.userContext.logout();
            this.$scope.$state.transitionTo("login");
        }

        //switchColorMode() {
        //    if (this.$scope.colorMode === 'light') {
        //        this.$scope.colorMode = 'dark';

        //        angular.element('html').removeClass('light-mode');
        //        angular.element('html').addClass('dark-mode');

        //        angular.element('nav.navbar').removeClass('white-bg');
        //        angular.element('nav.navbar').addClass('black-bg');

        //        angular.element('#page-wrapper').removeClass('gray-bg');
        //        angular.element('#page-wrapper').addClass('black-bg');
        //    }
        //    else if (this.$scope.colorMode === 'dark') {
        //        this.$scope.colorMode = 'light';

        //        angular.element('html').removeClass('dark-mode');
        //        angular.element('html').addClass('light-mode');

        //        angular.element('nav.navbar').removeClass('black-bg');
        //        angular.element('nav.navbar').addClass('white-bg');

        //        angular.element('#page-wrapper').removeClass('black-bg');
        //        angular.element('#page-wrapper').addClass('gray-bg');
        //    }
        //}
    }

    angular.module("app").controller("topnavbarController", TopnavbarController);
}