
module App {
    "use strict";

    interface IlmAccess extends ng.IDirective {
    }

    interface IlmAccessScope extends ng.IScope {
    }

    interface IlmAccessAttributes extends ng.IAttributes {
        lmAccess: string;
    }

    lmAccess.$inject = ['userContextService'];
    function lmAccess(userContext: Services.IUserContextService): IlmAccess {
        return {
            restrict: "A",
            link: link,
        };


        function link(scope: IlmAccessScope,
            element: ng.IAugmentedJQuery,
            attrs: IlmAccessAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            var makeVisible = function () {
                element.removeClass('hidden');
            };

            var makeHidden = function () {
                element.addClass('hidden');
            };

            var determineVisibility = function (roles: string[]) {
                var result;

                result = userContext.userIsInAnyRoles(roles);

                if (result) {
                    makeVisible();
                } else {
                    makeHidden();
                }
            };

            var roles = attrs.lmAccess.split(',');

            if (roles.length > 0) {
                determineVisibility(roles);
            }
        }
    }

    angular.module("app").directive("lmAccess", lmAccess);
}