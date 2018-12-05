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
            scope: {
                accountType: '@'
            },
            link: link,
        };


        function link(
            scope: IlmAccessScope,
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
                var result = userContext.userIsInAnyRoles(roles);
                if (result) {
                    makeVisible();
                } else {
                    makeHidden();
                }
            };

            var checkAccountType = function (type: string) {
                var accountTypes = ['mobile', 'web'];
                if (accountTypes.indexOf(type, 0) === -1)
                    makeHidden();
                else {
                    var accountType;
                    if (scope.accountType === 'mobile')
                        accountType = 0;
                    else if (scope.accountType === 'web')
                        accountType = 1;

                    if (userContext.current.orgUser === null) {
                        makeHidden();
                    } else {
                        if (userContext.current.orgUser.accountType === accountType && !userContext.userIsRestricted())
                            makeVisible();
                        else
                            makeHidden();
                    }
                }
            }

            if (scope.accountType !== undefined) {
                checkAccountType(scope.accountType);
            } else {
                var roles = attrs.lmAccess.split(',');
                if (roles.length > 0) {
                    determineVisibility(roles);
                }
            }
        }
    }

    angular.module("app").directive("lmAccess", lmAccess);
}