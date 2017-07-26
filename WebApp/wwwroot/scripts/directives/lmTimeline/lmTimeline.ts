module App {
    "use strict";

    interface IlmTimeline extends ng.IDirective {
    }

    interface IlmTimelineScope extends ng.IScope {
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        height: number;

        currentDate: Date;
        chartLabels: Date[];
        chartDatasets: any[];

        timelineChart: any;
    }

    interface IlmTimelineAttributes extends ng.IAttributes {
        
    }

    lmTimeline.$inject = [];
    function lmTimeline(): IlmTimeline {
        return {
            restrict: "E",
            replace: true,
            templateUrl: 'scripts/directives/lmTimeline/template.html',
            transclude: false,
            link: link,
            scope: {
                formTemplates: '=',
                surveys: '=',
                height: '@'
            }
        };

        function link(
            scope: IlmTimelineScope,
            element: ng.IAugmentedJQuery,
            attrs: IlmTimelineAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            element.css('height', scope.height + 'px');
            scope.currentDate = new Date();

            function generateTimelineData() {
                var daysInMonth = moment(scope.currentDate).daysInMonth();
                var currentDay = moment(scope.currentDate).date();
                var firstDayOfMonth = moment(scope.currentDate).add(-(currentDay - 1), 'day').toDate();
                var lastDayOfMonth = moment(scope.currentDate).add((daysInMonth - currentDay), 'day').toDate();

                var days = [];
                days.push(firstDayOfMonth);
                for (var i = 2; i <= daysInMonth; i++) {
                    var daysToAdd = -(currentDay - i);
                    var tick = moment(scope.currentDate).add(daysToAdd, 'day').toDate();
                    days.push(tick);
                }

                scope.chartLabels = days;

                var datasets = [];
                _.forEach(scope.formTemplates, (template) => {
                    var data = [];
                    var records = _.filter(scope.surveys, (survey) => { return survey.formTemplateId == template.id });

                    _.forEach(days, (day) => {
                        var foundSurveys = _.filter(records, (record) => {
                            if (moment(day).format('MM-DD-YYYY') === moment(record.surveyDate).format('MM-DD-YYYY')) {
                                return record;
                            }
                        });

                        if (foundSurveys.length) {
                            let impactSum = 0;
                            _.forEach(foundSurveys, (survey) => {
                                var timelineBarFormValue = _.filter(survey.formValues, { 'metricId': template.timelineBarMetricId })[0];
                                if (timelineBarFormValue) {
                                    impactSum += timelineBarFormValue.numericValue;
                                }
                            });

                            data.push(impactSum);
                        } else {
                            data.push(0);
                        }
                    });

                    var ds = {
                        label: template.title,
                        backgroundColor: template.colour,
                        borderColor: template.colour,
                        borderWidth: 2,
                        data: data,
                        stack: 1
                    };

                    datasets.push(ds);
                });

                scope.chartDatasets = datasets;

                renderTimelineChart();
            }

            function renderTimelineChart() {
                var canvas = <HTMLCanvasElement>element[0];
                var ctx = canvas.getContext('2d');

                var chartOptions = {
                    responsive: true,
                    maintainAspectRatio: false,
                    tooltips: {
                        mode: 'index',
                        callbacks: {
                            title: function (items, data) {
                                var xLabel = items[0].xLabel;
                                var yValue = 0;
                                _.forEach(items, (item) => {
                                    yValue += parseInt(item.yLabel);
                                });

                                var result = [];
                                result.push(xLabel);
                                result.push(`Impact: ${yValue}`);

                                return result;
                            },
                            label: function (item, data) {
                                var label = data.datasets[item.datasetIndex].label;
                                return `${label}: ${item.yLabel}`;
                            }
                        }
                    },
                    scales: {
                        xAxes: [{
                            display: true,
                            time: {
                                unit: 'day',
                                displayFormats: {
                                    day: 'lll'
                                }
                            },
                            ticks: {
                                autoSkip: true,
                                callback: function (value) {
                                    return moment(value).format('MMM D');
                                },
                            }
                        }],
                        yAxes: [{
                            ticks: {
                                beginAtZero: true,
                                max: 100
                            }
                        }]
                    }
                };

                var config = {
                    type: 'groupableBar',
                    data: {
                        labels: scope.chartLabels,
                        datasets: scope.chartDatasets
                    },
                    options: chartOptions
                };

                if (scope.timelineChart)
                    scope.timelineChart.destroy();

                scope.timelineChart = new Chart(ctx, config);
            }

            scope.$watchGroup(['formTemplates', 'surveys'], () => {
                generateTimelineData();    
            });
        }
    }

    angular.module("app").directive("lmTimeline", lmTimeline);
}