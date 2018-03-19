module App {
    "use strict";

    interface IlmAtWho extends ng.IDirective {
    }

    interface IlmAtWhoScope extends ng.IScope {
        data: any[];
        placeholder: string;
    }

    lmAtWho.$inject = ['userContextService'];
    function lmAtWho(): IlmAtWho {
        return {
            restrict: "E",
            replace: true,
            templateUrl: 'scripts/directives/lmAtWho/template.html',
            transclude: false,
            link: link,
            scope: {
                data: '=',
                placeholder: '@'
            }
        };

        function link(scope: IlmAtWhoScope,
            element: ng.IAugmentedJQuery,
            attrs: ng.IAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            var dataset = [];
            _.forEach(scope.data, (item, index) => {
                dataset.push({ id: index + 1, name: _.toLower(item) });
            });

            var config = {
                at: '@',
                data: dataset,
                displayTpl: "<li>${name} </li>",
                insertTpl: '{{${name}}}',
            };

            $(element).atwho(config);
        }
    }

    angular.module("app").directive("lmAtWho", lmAtWho);
}