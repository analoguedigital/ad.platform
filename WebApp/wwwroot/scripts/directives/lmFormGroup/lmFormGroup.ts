
module App {
    "use strict";
    //https://github.com/lawrence0819/angular-bootstrap-form-group/blob/master/src/angular-bootstrap-form-group.coffee

    interface IlmFormGroup extends ng.IDirective {
    }

    interface IlmFormGroupScope extends ng.IScope {
        label: string;
        for: string;
        form: any;
        hasError: boolean;
        formDirty: boolean;
        isFormHorizontal: boolean;
        errors: any[];
    }

    interface IlmFormGroupAttributes extends ng.IAttributes {
    }

    lmFormGroup.$inject = ['$injector'];
    function lmFormGroup($injector: ng.auto.IInjectorService): IlmFormGroup {
        return {
            restrict: "E",
            require: '^form',
            replace: true,
            templateUrl: "scripts/directives/lmFormGroup/template.html",
            transclude: true,
            link: link,
            scope: {
                label: "@",
                formGroupClass: '@'
            }
        };


        function link(scope: IlmFormGroupScope,
            element: ng.IAugmentedJQuery,
            attrs: IlmFormGroupAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            transclude((inputElement) => {
                return element.find('span').replaceWith(inputElement);
            });

            if (element.closest('form').hasClass('form-horizontal')) {
                scope.isFormHorizontal = true;
            }

            var inputTagNames: string[] = ['input', 'select', 'textarea'];
            var $input = null;

            for (var i = 0, len = inputTagNames.length; i < len; i++) {
                var tagName = inputTagNames[i];
                $input = element.find(tagName);
                if ($input.length > 0) {
                    break;
                }
            }
            if ($input.length === 0) {
                return;
            }

            $input.addClass("form-control");

            var inputId = $input.attr("id");
            var inputName = $input.attr("name");

            if (inputName) {
                var form = element.closest('form');
            }

            var formName = form.attr('name');
            scope.for = inputId;

            var invalidExpression = [formName, inputName, '$invalid'].join('.');
            var dirtyExpression = [formName, inputName, '$touched'].join('.');
            var errorExpression = [formName, inputName, '$error'].join('.');

            scope.$parent.$watch(invalidExpression, function (hasError: boolean) {
                return scope.hasError = hasError;
            });

            scope.$parent.$watch(dirtyExpression, function (dirty: boolean) {
                return scope.formDirty = dirty;
            });

            return scope.$parent.$watch(errorExpression, function (errors) {
                var $translate, fn, message, notValid, _results;
                scope.errors = [];
                _results = [];
                for (var property in errors) {
                    if (errors.hasOwnProperty(property)) {

                        notValid = errors[property];
                        message = $input.attr('ng-em-' + property);
                        if (!message) {
                            message = property;
                        }
                        if ($injector.has('$translate')) {
                            $translate = $injector.get('$translate');
                            fn = function (messageKey, value, property, notValid) {
                                return $translate(messageKey, {
                                    value: value
                                }).then(function (translatedMessage) {
                                    return scope.errors.push({
                                        name: property,
                                        message: translatedMessage,
                                        notValid: notValid
                                    });
                                });
                            };
                            _results.push(fn(message, $input.attr('ng-' + property) || $input.attr(property), property, notValid));
                        } else {
                            _results.push(scope.errors.push({
                                name: property,
                                message: message,
                                notValid: notValid
                            }));
                        }
                    }
                }
                return _results;
            }, true);
            
            //scope.$parent.$watch(input + '.$invalid && ' + input + '.$touched', function (hasError: boolean) {
            //    scope.hasError = hasError;
            //});
        }
    }

    angular.module("app").directive("lmFormGroup", lmFormGroup);
}