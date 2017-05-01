
module App {
    "use strict";

    interface INavigationControllerScope extends ng.IScope {
        title: string;
        user: string;
        $state: ng.ui.IStateService;
        logout: () => void;
        resetPassword: () => void;
    }

    interface INavigationController {
        activate: () => void;
        logout: () => void;
    }

    class NavigationController implements INavigationController {
        static $inject: string[] = ["$scope", "$uibModal", "userContextService"];

        constructor(
            private $scope: INavigationControllerScope,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private userContextService: Services.IUserContextService
        ) {

            $scope.title = "navigation";
            $scope.user = userContextService.current.orgUser == null ?
                userContextService.current.user.email :
                userContextService.current.orgUser.toString();
            $scope.logout = () => { this.logout(); };
            $scope.resetPassword = () => { this.resetPassword(); };

            this.activate();
        }

        activate() {
            //
        }

        resetPassword() {

            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/common/resetPassword/resetPasswordView.html',
                controller: 'resetPasswordController',
                controllerAs: 'ctrl',
                resolve: {
                    user: () => { return null;}
                }
            });
        }

        logout() {
            this.userContextService.logout();
            this.$scope.$state.transitionTo("login");
        }
    }

    angular.module("app").controller("navigationController", NavigationController);
}