
module App {
    "use strict";

    interface IFooterControllerScope extends ng.IScope {
    }

    interface IFooterController {
        activate: () => void;
    }

    class FooterController implements IFooterController {
        static $inject: string[] = ["$scope", "userContextService"];

        constructor(
            private $scope: IFooterControllerScope,
            private userContext: Services.IUserContextService) {

            this.activate();
        }

        activate() {
            //
        }
    }

    angular.module("app").controller("footerController", FooterController);
}