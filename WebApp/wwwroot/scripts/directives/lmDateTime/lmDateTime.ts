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

            let dateValue = new Date(scope.value);
            let hours = dateValue.getHours();
            let minutes = dateValue.getMinutes();
            let result: string;

            if (hours > 0 || minutes > 0)
                result = moment(scope.value).format('DD/MM/YYYY hh:mm A');
            else
                result = moment(scope.value).format('DD/MM/YYYY');

            scope.resultString = result;
        }
    }

    angular.module("app").directive("lmDateTime", lmDateTime);
}