module App {
    "use strict";

    interface IlmTimeline extends ng.IDirective {
    }

    interface IlmTimelineScope extends ng.IScope {
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];

        height: number;
        backgroundColor: string;

        currentDate: Date;
        chartLabels: Date[];
        chartDatasets: any[];

        timelineChart: any;
    }

    interface IlmTimelineAttributes extends ng.IAttributes {

    }

    lmTimeline.$inject = ['$rootScope'];
    function lmTimeline($rootScope: ng.IRootScopeService): IlmTimeline {
        return {
            restrict: "E",
            replace: true,
            templateUrl: 'scripts/directives/lmTimeline/template.html',
            transclude: false,
            link: link,
            scope: {
                formTemplates: '=',
                surveys: '=',
                height: '@',
                backgroundColor: '@'
            }
        };

        function link(
            scope: IlmTimelineScope,
            element: ng.IAugmentedJQuery,
            attrs: IlmTimelineAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            if (scope.height)
                element.css('height', scope.height + 'px');
            if (scope.backgroundColor)
                element.css('background-color', scope.backgroundColor);

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
                            barThickness: 20,
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
                    },
                    hover: {
                        animationDuration: 0
                    },
                    animation: {
                        duration: 1,
                        onComplete: function () {
                            var chartInstance = this.chart;
                            var ctx = chartInstance.ctx;

                            ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontSize, Chart.defaults.global.defaultFontStyle, Chart.defaults.global.defaultFontFamily);
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';

                            this.data.datasets.forEach(function (dataset, i) {
                                var meta = chartInstance.controller.getDatasetMeta(i);

                                if (meta.hidden === null || meta.hidden === false) {
                                    meta.data.forEach(function (bar, index) {
                                        var data = dataset.data[index];
                                        var impact = parseInt(data);

                                        if (impact > 0) {
                                            var centerX = bar._model.x;
                                            var centerY = bar._model.y - 5;
                                            var radius = 10;

                                            ctx.beginPath();
                                            ctx.arc(centerX, centerY, radius, 0, 2 * Math.PI, false);
                                            ctx.fillStyle = 'white';
                                            ctx.fill();
                                            ctx.lineWidth = 1;
                                            ctx.strokeStyle = 'white';
                                            ctx.stroke();

                                            ctx.fillStyle = dataset.backgroundColor;
                                            ctx.fillText(data, bar._model.x, bar._model.y + 2);
                                        }
                                    });
                                }
                            });
                        }
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

            scope.nextMonth = function () {
                scope.currentDate = moment(scope.currentDate).add(1, 'months').toDate();
            }

            scope.previousMonth = function () {
                scope.currentDate = moment(scope.currentDate).subtract(1, 'months').toDate();
            }

            scope.$watchGroup(['formTemplates', 'surveys'], () => {
                generateTimelineData();
                renderTimelineChart();
            });

            scope.$watch('currentDate', () => {
                generateTimelineData();
                renderTimelineChart();
            });

            $rootScope.$on('timeline-next-month', () => {
                scope.nextMonth();
            });
            $rootScope.$on('timeline-previous-month', () => {
                scope.previousMonth();
            });
        }
    }

    angular.module("app").directive("lmTimeline", lmTimeline);
}