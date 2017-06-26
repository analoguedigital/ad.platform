﻿module App {
    "use strict";

    interface IlmPieChart extends ng.IDirective {

    }

    interface IlmPieChartScope extends ng.IScope {
        id: string;
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
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
                formTemplates: '=',
                surveys: '='
            }
        };
        
        function link(scope: IlmPieChartScope,
            element: ng.IAugmentedJQuery,
            attrs: IlmPieChartAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            scope.options = {
                type: 'pie',
                responsive: false,
                maintainAspectRatio: true,
                showTooltips: true,
                legend: {
                    display: true,
                    position: 'bottom'
                }
            };

            scope.$watchGroup(['formTemplates', 'surveys'], () => {
                scope.data = [];
                scope.labels = [];
                scope.colors = [];

                angular.forEach(scope.formTemplates, (template) => {
                    if (_.filter(scope.surveys, (survey) => { return survey.formTemplateId == template.id }).length) {
                        scope.labels.push(template.title);
                        scope.colors.push(template.colour);

                        let records = _.filter(scope.surveys, (survey) => { return survey.formTemplateId == template.id; });
                        scope.data.push(records.length);
                    }
                }); 
            });
        }
    }

    angular.module("app").directive("lmPieChart", lmPieChart);
}