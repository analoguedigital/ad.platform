module App {
    "use strict";

    interface IDataSetItem {
        id: number;
        serial: number;
    }

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

            //scope.$watch('data', (newValue, oldValue) => {
                //_.forEach(newValue, (item, index) => {
                //    dataset.push({ id: index + 1, serial: item });
                //});
            //});

            var dataset: IDataSetItem[] = [];

            _.forEach(scope.data, (item, index) => {
                dataset.push({ id: index + 1, serial: item });
            });

            var config = {
                at: '#',
                data: dataset,
                limit: dataset.length,
                maxLen: 10,
                startWithSpace: false,
                searchKey: 'serial',
                displayTpl: "<li>#${serial}</li>",
                insertTpl: '#${serial},',

                callbacks: {
                    beforeInsert: (value, $li) => {
                        var inputValue = $(element).val();

                        var pattern = new RegExp(value, "gm");
                        var result = inputValue.match(pattern);

                        if (result == null)
                            return value;

                        return '';

                        //var serial = value.substr(1, value.length - 2);
                        //_.remove(dataset, (item: any) => { return item.serial == serial; });
                    }
                }
            };

            $(element).atwho(config);
        }
    }

    angular.module("app").directive("lmSerialReferences", lmSerialReferences);
}