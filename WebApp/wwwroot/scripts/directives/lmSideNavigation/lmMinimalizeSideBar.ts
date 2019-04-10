﻿
module App {
    "use strict";

    interface IlmMinimalizeSideBar extends ng.IDirective {
    }

    interface IlmMinimalizeSideBarScope extends ng.IScope {
    }

    interface IlmMinimalizeSideBarAttributes extends ng.IAttributes {
    }

    lmMinimalizeSideBar.$inject = ["$timeout", "localStorageService"];
    function lmMinimalizeSideBar($timeout: ng.ITimeoutService, localStorageService: ng.local.storage.ILocalStorageService): IlmMinimalizeSideBar {
        return {
            restrict: "A",
            template: '<a class="navbar-minimalize minimalize-styl-2 btn btn-primary " href="" ng-click="minimalize()">'
            + '<i class="fa fa-bars"></i></a>',
            controller: function ($scope, $element) {
                $scope.minimalize = function () {
                    angular.element('body').toggleClass('mini-navbar');
                    if (!angular.element('body').hasClass('mini-navbar') || angular.element('body').hasClass('body-small')) {
                        // Hide menu in order to smoothly turn on when maximize menu
                        angular.element('#side-menu').hide();
                        // For smoothly turn on menu
                        $timeout(function () {
                            angular.element('#side-menu').fadeIn(500);
                        }, 100);

                        localStorageService.set('sidenav_toggle', true);
                    } else {
                        // Remove all inline style from jquery fadeIn function to reset menu state
                        angular.element('#side-menu').removeAttr('style');
                        localStorageService.set('sidenav_toggle', false);
                    }
                };
            },
            link: link
        };

        function link(scope: IlmMinimalizeSideBarScope, element: ng.IAugmentedJQuery, attrs: IlmMinimalizeSideBarAttributes) {
            var toggle = localStorageService.get('sidenav_toggle');
            if (toggle !== null && !toggle) {
                angular.element('body').toggleClass('mini-navbar');
            }
        }
    }

    angular.module("app").directive("lmMinimalizeSideBar", lmMinimalizeSideBar);
}