
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

        static $inject: string[] = ["$scope", "$rootScope", "$state", "userContextService"];
        constructor(private $scope: IIndexControllerScope,
            private $rootScope: IIndexControllerRootScope,
            private $state: angular.ui.IStateService,
            userContext: App.Services.IUserContextService) {

            $scope.title = "Index";
            if ($state.current.data)
                $rootScope.bodyCssClass = $state.current.data.bodyCssClass;

            this.activate(userContext.current.user !== null);
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