declare var moment: any;

module App {
    "use strict";
    //https://github.com/lawrence0819/angular-bootstrap-form-group/blob/master/src/angular-bootstrap-form-group.coffee

    interface IlmDateTime extends ng.IDirective {
    }

    interface IlmDateTimeScope extends ng.IScope {
        value: Date;
        resultString: string;
    }

    lmDateTime.$inject = ['$injector'];
    function lmDateTime($injector: ng.auto.IInjectorService): IlmDateTime {
        return {
            restrict: "E",
            replace: true,
            template: '<span>{{resultString}}</span>',
            link: link,
            scope: {
                value: "="
            }
        };

        function link(
            scope: IlmDateTimeScope,
            element: ng.IAugmentedJQuery,
            ctrl: any) {

            if (scope.value) {
                var hours = scope.value.getHours();
                var minutes = scope.value.getMinutes();
                let formatString = (hours > 0 || minutes > 0) ? 'L LT' : 'L';
                let result = moment(scope.value).format(formatString);

                scope.resultString = result;
            }
        }
    }

    angular.module("app").directive("lmDateTime", lmDateTime);
}