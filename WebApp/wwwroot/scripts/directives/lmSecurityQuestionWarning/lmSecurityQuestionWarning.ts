module App {
    "use strict";

    interface IlmSecurityQuestionWarning extends ng.IDirective {
    }

    interface IlmSecurityQuestionWarningScope extends ng.IScope {
    }

    interface IlmSecurityQuestionWarningAttributes extends ng.IAttributes {

    }

    lmSecurityQuestionWarning.$inject = ['$injector', 'userContextService'];
    function lmSecurityQuestionWarning($injector: ng.auto.IInjectorService, userContextService: App.Services.IUserContextService): IlmSecurityQuestionWarning {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'scripts/directives/lmSecurityQuestionWarning/template.html',
            transclude: false,
            link: link,
            scope: {
            }
        };

        function link(scope: IlmSecurityQuestionWarningScope,
            element: ng.IAugmentedJQuery,
            attrs: IlmSecurityQuestionWarningAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {
            scope.securityQAEnabled = userContextService.current.user.securityQuestionEnabled;
        }
    }

    angular.module("app").directive("lmSecurityQuestionWarning", lmSecurityQuestionWarning);
}