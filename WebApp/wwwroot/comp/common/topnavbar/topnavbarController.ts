
module App {
    "use strict";

    interface ITopnavbarControllerScope extends ng.IScope {
        title: string;
        user: string;
        $state: ng.ui.IStateService;
        logout: () => void;
    }

    interface ITopnavbarController {
        activate: () => void;
        logout: () => void;
    }

    class TopnavbarController implements ITopnavbarController {

        static $inject: string[] = ["$scope", "userContextService"];
        constructor(
            private $scope: ITopnavbarControllerScope,
            private userContext: Services.IUserContextService) {

            $scope.title = "top navbar";
            $scope.user = userContext.current.user.email;
            $scope.logout = () => { this.logout(); };

            this.activate();
        }

        activate() {
            //
        }

        logout() {
            this.userContext.logout();
            this.$scope.$state.transitionTo("login");
        }
    }

    angular.module("app").controller("topnavbarController", TopnavbarController);
}