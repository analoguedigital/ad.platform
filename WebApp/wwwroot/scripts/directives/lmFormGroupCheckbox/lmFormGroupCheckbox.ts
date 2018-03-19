module App {
    "use strict";

    interface IlmFormGroupCheckbox extends ng.IDirective {
    }

    interface IlmFormGroupCheckboxScope extends ng.IScope {
        label: string;
        for: string;
        form: any;
        hasError: boolean;
    }

    interface IlmFormGroupCheckboxAttributes extends ng.IAttributes {
    }

    lmFormGroupCheckbox.$inject = [];
    function lmFormGroupCheckbox(): IlmFormGroupCheckbox {
        return {
            restrict: "E",
            require: '^form',
            replace: true,
            templateUrl: "scripts/directives/lmFormGroupCheckbox/template.html",
            transclude: true,
            link: link,
            scope: {
                label: "@",
            }
        };

        function link(scope: IlmFormGroupCheckboxScope, element: ng.IAugmentedJQuery, attrs: IlmFormGroupCheckboxAttributes, ctrl: any) {
            var $input = element.find("input");

            scope.for = $input.attr("id");
            $input.addClass("form-control");

            scope.form = ctrl;
            var input = scope.form.$name + '.' + $input.attr("name");

            scope.$parent.$watch(input + '.$invalid && ' + input + '.$touched', function (hasError: boolean) {
                scope.hasError = hasError;
            });
        }
    }

    angular.module("app").directive("lmFormGroupCheckbox", lmFormGroupCheckbox);
}