module App {
    "use strict";

    interface IlmPieChart extends ng.IDirective {

    }

    interface IlmPieChartScope extends ng.IScope {
        id: string;
        data: number[];
        labels: string[];
        colors: string[];
        options: any;
    }

    interface IlmPieChartAttributes extends ng.IAttributes {

    }

    function lmPieChart($injector: ng.auto.IInjectorService): IlmPieChart {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'scripts/directives/lmPieChart/template.html',
            transclude: false,
            link: link,
            scope: {
                id: '@',
                data: '=',
                labels: '=',
                colors: '='
            }
        };

        function link(scope: IlmPieChartScope,
            element: ng.IAugmentedJQuery,
            attrs: IlmPieChartAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            scope.options = {
                type: 'pie',
                responsive: true,
                maintainAspectRatio: true,
                showTooltips: true,
                legend: {
                    display: true,
                    position: 'bottom'
                }
            }
        }
    }

    angular.module("app").directive("lmPieChart", lmPieChart);
}