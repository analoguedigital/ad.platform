
module App {
    "use strict";

    interface IIndexControllerScope extends ng.IScope {
        title: string;
    }

    interface IIndexControllerRootScope extends ng.IRootScopeService {
        bodyCssClass: string;
    }

    interface IIndexController {
        activate: (isAuthenticated: boolean) => void;
    }

    class IndexController implements IIndexController {

        static $inject: string[] = ["$scope", "userContextService"];
        constructor(
            private $scope: IIndexControllerScope,
            private userContextService: Services.IUserContextService) {

            $scope.title = "Index";

            this.activate(userContextService.current.user !== null);
        }

        activate(isAuthenticated: boolean) {
            //if (isAuthenticated === undefined || !isAuthenticated) {
            //    this.$state.tra .go('login');
            //} else {
            //    this.$stateService.go('home');
            //}
        }
    }

    angular.module("app").controller("indexController", IndexController);
}