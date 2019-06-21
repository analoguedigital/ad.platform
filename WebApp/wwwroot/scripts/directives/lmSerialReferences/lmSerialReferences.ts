module App {
    "use strict";

    interface IlmSerialReferences extends ng.IDirective {
    }

    interface IlmSerialReferencesScope extends ng.IScope {
        data: any[];
        placeholder: string;
    }

    lmSerialReferences.$inject = ['userContextService'];
    function lmSerialReferences(): IlmSerialReferences {
        return {
            restrict: "E",
            replace: true,
            templateUrl: 'scripts/directives/lmSerialReferences/template.html',
            transclude: false,
            link: link,
            scope: {
                data: '=',
                placeholder: '@'
            }
        };

        function link(scope: IlmSerialReferencesScope,
            element: ng.IAugmentedJQuery,
            attrs: ng.IAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            scope.$watch('data', (newValue, oldValue) => {
                var dataset = [];

                _.forEach(newValue, (item, index) => {
                    dataset.push({ id: index + 1, serial: item });
                });

                var config = {
                    at: '#',
                    data: dataset,
                    limit: dataset.length,
                    maxLen: 50,
                    startWithSpace: false,
                    suffix: ',',
                    searchKey: 'serial',
                    displayTpl: "<li>#${serial}</li>",
                    insertTpl: '#${serial}'
                };

                $(element).atwho(config);
            });
        }
    }

    angular.module("app").directive("lmSerialReferences", lmSerialReferences);
}