
module App {
    "use strict";

    interface IlmSideBar extends ng.IDirective {
    }

    interface IlmSideBarScope extends ng.IScope {
    }

    interface IlmSideBarAttributes extends ng.IAttributes {
    }

    lmSideBar.$inject = ["$timeout"];
    function lmSideBar($timeout: ng.ITimeoutService): IlmSideBar {
        return {
            restrict: "A",
            link: link
        };

        function link(scope: IlmSideBarScope, element: ng.IAugmentedJQuery, attrs: IlmSideBarAttributes) {

            $timeout(function () {
                element.metisMenu();
            });
        }
    }

    angular.module("app").directive("lmSideBar", lmSideBar);
}