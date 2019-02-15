module App {
    "use strict";

    export interface IProjectSummaryPrintScope extends ng.IScope {
        ctrl: IProjectSummaryPrintController;
    }

    interface IProjectSummaryPrintController {
        title: string;
        project: Models.IProject;
        formTemplates: Array<Models.IFormTemplate>;
        surveys: Array<Models.ISurvey>;
        uniqFormTemplates: Models.IFormTemplate[];
        locationCount: number;

        activate: () => void;
        downloadPdf: () => void;
        downloadZip: () => void;

        showMap: boolean;
        showPieChart: boolean;
        showTimeline: boolean;
        enableSnapshotView: boolean;

        totalFormTemplates: number;
        totalSurveys: number;
        totalImpact: number;
    }

    class ProjectSummaryPrintController implements IProjectSummaryPrintController {
        title: string = "Forms";
        project: Models.IProject;
        formTemplates: Array<Models.IFormTemplate> = [];
        surveys: Array<Models.ISurvey> = [];
        uniqFormTemplates: Models.IFormTemplate[];
        locationCount: number;
        showMap: boolean = false;
        showPieChart: boolean = false;
        showTimeline: boolean = true;
        enableSnapshotView: boolean = true;
        totalFormTemplates: number;
        totalSurveys: number;
        totalImpact: number = 0;

        mapCenterValue: any;
        mapZoomValue: number;
        mapTypeValue: string;

        private TIMELINE_TOGGLE_KEY: string = 'show_timeline';
        private TIMELINE_STATE_KEY: string = 'timeline_state';
        private MAP_TOGGLE_KEY: string = 'show_map';
        private PIE_CHART_TOGGLE_KEY: string = 'show_pie_chart';

        static $inject: string[] = ["$http", "$scope", "$rootScope", "$state", "$stateParams", "$q", "$location",
            "projectSummaryPrintSessionResource", "formTemplateResource", "surveyResource", "projectResource",
            "localStorageService", "session"];
        constructor(
            private $http: ng.IHttpService,
            private $scope: IProjectSummaryPrintScope,
            private $rootScope: ng.IRootScopeService,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $q: ng.IQService,
            private $location: ng.ILocationService,
            private projectSummaryPrintSessionResource: App.Resources.IProjectSummaryPrintSessionResource,
            private formTemplateResource: App.Resources.IFormTemplateResource,
            private surveyResource: App.Resources.ISurveyResource,
            private projectResource: App.Resources.IProjectResource,
            private localStorageService: ng.local.storage.ILocalStorageService,
            private session: App.Models.IProjectSummaryPrintSession) {

            this.activate();
        }

        activate() {
            if (this.session == null) {
                // session has timed out. redirect to home.
                this.$state.go('home.dashboard.layout');
                return;
            }

            var self = this;

            var _timeline = this.$location.search().timeline;
            var _locations = this.$location.search().locations;
            var _pieChart = this.$location.search().piechart;
            var _latitude = this.$location.search().lat;
            var _longitude = this.$location.search().lng;
            var _zoomLevel = this.$location.search().zoomLevel;
            var _mapType = this.$location.search().mapType;

            if (_timeline && _timeline.length) {
                self.showTimeline = _.toLower(_timeline) === 'true' ? true : false;
            }
            else {
                var toggle = this.localStorageService.get(this.TIMELINE_TOGGLE_KEY);
                if (toggle !== null)
                    self.showTimeline = <boolean>toggle;
            }

            if (_locations && _locations.length)
                self.showMap = _.toLower(_locations) === 'true' ? true : false;
            else {
                var toggle = this.localStorageService.get(this.MAP_TOGGLE_KEY);
                if (toggle !== null)
                    self.showMap = <boolean>toggle;
            }

            if (_pieChart && _pieChart.length)
                self.showPieChart = _.toLower(_pieChart) === 'true' ? true : false;
            else {
                var toggle = this.localStorageService.get(this.PIE_CHART_TOGGLE_KEY);
                if (toggle !== null)
                    self.showPieChart = <boolean>toggle;
                else
                    self.showPieChart = false;
            }

            var _timelineState = this.localStorageService.get(this.TIMELINE_STATE_KEY);
            if (_timelineState !== null)
                self.enableSnapshotView = <boolean>_timelineState;
            else
                self.enableSnapshotView = true;

            this.$rootScope.$on('timeline-in-snapshot-view', () => {
                this.enableSnapshotView = true;
            });

            this.$rootScope.$on('timeline-in-month-view', () => {
                this.enableSnapshotView = false;
            });

            this.$scope.today = new Date();
            this.$scope.months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

            let formTemplates = [];
            let surveys = [];
            let promises = [];

            angular.forEach(this.session.surveyIds, (surveyId) => {
                let deferred = this.$q.defer();
                promises.push(deferred.promise);

                this.surveyResource.get({ id: surveyId }).$promise
                    .then((survey) => {
                        var impactPattern = new RegExp(/( - Impact )\w+/g);
                        var surveyDescription = survey.description.replace(impactPattern, '');
                        survey.description = surveyDescription;

                        surveys.push(survey);

                        this.formTemplateResource.get({ id: survey.formTemplateId }).$promise
                            .then((data) => {
                                formTemplates.push(data);
                                deferred.resolve();
                            });
                    });
            });

            let deferred = this.$q.defer();
            promises.push(deferred);

            this.projectResource.get({ id: this.session.projectId }).$promise.then((data) => {
                this.project = data;
                deferred.resolve();
            });

            this.$q.all(promises).then(() => {
                this.surveys = _.sortBy(surveys, ['date', 'formTemplateId']);
                this.formTemplates = formTemplates;
                this.uniqFormTemplates = _.uniqBy(this.formTemplates, (t) => { return t.id });

                this.totalFormTemplates = this.uniqFormTemplates.length;
                this.totalSurveys = this.surveys.length;

                _.forEach(this.uniqFormTemplates, (template) => {
                    this.totalImpact += this.getTotalImpact(template);
                });

                // coming from the project summary page
                if (this.$stateParams.mapZoomLevel > 0 && !isNaN(this.$stateParams.mapCenter.latitude) && !isNaN(this.$stateParams.mapCenter.longitude) && this.$stateParams.mapType.length) {
                    this.mapCenterValue = this.$stateParams.mapCenter;
                    this.mapZoomValue = this.$stateParams.mapZoomLevel;
                    this.mapTypeValue = this.$stateParams.mapType;

                    this.localStorageService.set('export_map_center', this.$stateParams.mapCenter);
                    this.localStorageService.set('export_map_zoom_level', this.$stateParams.mapZoomLevel);
                    this.localStorageService.set('export_map_type', this.$stateParams.mapType);

                    this.$rootScope.$broadcast('update_locations_map', {
                        center: this.$stateParams.mapCenter,
                        zoomLevel: this.$stateParams.mapZoomLevel,
                        mapType: this.$stateParams.mapType
                    });
                } else if (_latitude && _longitude && _zoomLevel && _mapType) {
                    // pdf/zip export requests, calling this URL from server code
                    var latValue = Number(_latitude).valueOf();
                    var lngValue = Number(_longitude).valueOf();
                    var zoomValue = Number(_zoomLevel).valueOf();
                    var mapType = _mapType;

                    this.mapCenterValue = { latitude: latValue, longitude: lngValue };
                    this.mapZoomValue = zoomValue;
                    this.mapTypeValue = mapType;

                    this.$rootScope.$broadcast('update_locations_map', {
                        center: { latitude: latValue, longitude: lngValue },
                        zoomLevel: zoomValue,
                        mapType: mapType
                    });
                } else {
                    var _lsZoomLevel = this.localStorageService.get('export_map_zoom_level');
                    var _lsCenter = this.localStorageService.get('export_map_center');
                    var _lsMapType = this.localStorageService.get('export_map_type');

                    if (_lsZoomLevel !== null && _lsCenter !== null && _lsMapType !== null) {
                        this.mapCenterValue = _lsCenter;
                        this.mapZoomValue = <number>_lsZoomLevel;
                        this.mapTypeValue = <string>_lsMapType;

                        this.$rootScope.$broadcast('update_locations_map', {
                            center: _lsCenter,
                            zoomLevel: _lsZoomLevel,
                            mapType: _lsMapType
                        });
                    }
                }
            });
        }

        getTotalImpact(template: Models.IFormTemplate) {
            var totalValue = 0;

            if (template.timelineBarMetricId && template.timelineBarMetricId.length) {
                var records = _.filter(this.surveys, (s) => { return s.formTemplateId == template.id; });
                _.forEach(records, (r) => {
                    var impactValues = _.filter(r.formValues, (fv) => { return fv.metricId == template.timelineBarMetricId; });
                    if (impactValues.length) {
                        totalValue += impactValues[0].numericValue;
                    }
                });
            }

            return totalValue;
        }

        getSurveyImpact(survey: Models.ISurvey) {
            var impactValue = 0;

            var templates = _.filter(this.uniqFormTemplates, (t) => { return t.id == survey.formTemplateId; });
            if (templates && templates.length) {
                var template = templates[0];
                if (template.timelineBarMetricId && template.timelineBarMetricId.length) {
                    var formValues = _.filter(survey.formValues, (fv) => { return fv.metricId == template.timelineBarMetricId; });
                    if (formValues && formValues.length) {
                        impactValue = formValues[0].numericValue;
                    }
                }
            }

            return impactValue;
        }

        getFormTemplate(formTemplateId: string) {
            return _.find(this.formTemplates, { "id": formTemplateId });
        }

        deleteItem(id: string) {
            this.session.removedItemIds.push(id);
        }

        isItemVisible(itemId: string) {
            return this.session.removedItemIds.indexOf(itemId) === -1;
        }

        resetView() {
            this.session.removedItemIds = [];
            //this.showTimeline = true;
            //this.showMap = true;
            //this.showPieChart = true;
        }

        downloadPdf() {
            this.session.$save().then(() => {
                var requestUrl = "/api/projectSummaryPrintSession/downloadPdf/" +
                    this.session.id + "/" + this.showTimeline + "/" + this.showMap + "/" + this.showPieChart +
                    "/" + this.mapCenterValue.latitude + "/" + this.mapCenterValue.longitude +
                    "/" + this.mapZoomValue + "/" + this.mapTypeValue;

                this.$http.get(requestUrl, { responseType: 'arraybuffer' }).then((result) => {
                    let headers = result.headers();

                    var filename = headers['x-filename'];
                    var contentType = headers['content-type'];

                    var linkElement = document.createElement('a');
                    try {
                        var blob = new Blob([result.data], { type: contentType });
                        var url = window.URL.createObjectURL(blob);

                        linkElement.setAttribute('href', url);
                        linkElement.setAttribute("download", filename);

                        var clickEvent = new MouseEvent("click", {
                            "view": window,
                            "bubbles": true,
                            "cancelable": false
                        });
                        linkElement.dispatchEvent(clickEvent);
                    } catch (ex) {
                        console.log(ex);
                    }
                });
            });
        }

        downloadZip() {
            this.session.$save().then(() => {
                this.$http.get("/api/projectSummaryPrintSession/downloadZip/" + this.session.id + "/" + this.showTimeline + "/" +
                    this.showMap + "/" + this.showPieChart + "/" + this.mapCenterValue.latitude + "/" + this.mapCenterValue.longitude +
                    "/" + this.mapZoomValue + "/" + this.mapTypeValue, { responseType: 'arraybuffer' }).then((result) => {

                    let headers = result.headers();

                    var filename = headers['x-filename'];
                    var contentType = headers['content-type'];

                    var linkElement = document.createElement('a');
                    try {
                        var blob = new Blob([result.data], { type: contentType });
                        var url = window.URL.createObjectURL(blob);

                        linkElement.setAttribute('href', url);
                        linkElement.setAttribute("download", filename);

                        var clickEvent = new MouseEvent("click", {
                            "view": window,
                            "bubbles": true,
                            "cancelable": false
                        });
                        linkElement.dispatchEvent(clickEvent);
                    } catch (ex) {
                        console.log(ex);
                    }
                });
            });
        }

        addFormValue(metric, rowDataListItem, rowNumber) {
            var formValue = <Models.IFormValue>{};
            formValue.textValue = '';
            formValue.metricId = metric.id;
            formValue.rowNumber = rowNumber;
            if (rowDataListItem)
                formValue.rowDataListItemId = rowDataListItem.id;

            // no need to add the new formValue to the surveys as we are in the print mode 
            return formValue;
        }

        toggleTimeline() {
            this.showTimeline = !this.showTimeline;
            this.localStorageService.set(this.TIMELINE_TOGGLE_KEY, this.showTimeline);
        }

        toggleMap() {
            this.showMap = !this.showMap;
            this.localStorageService.set(this.MAP_TOGGLE_KEY, this.showMap);
        }

        togglePieChart() {
            this.showPieChart = !this.showPieChart;
            this.localStorageService.set(this.PIE_CHART_TOGGLE_KEY, this.showPieChart);
        }

        toggleSnapshotView() {
            this.enableSnapshotView = !this.enableSnapshotView;
            this.localStorageService.set(this.TIMELINE_STATE_KEY, this.enableSnapshotView);
        }

        timelineNextMonth() {
            this.$scope.today = moment(this.$scope.today).add(1, 'months').toDate();
            this.$rootScope.$broadcast('timeline-next-month');
        }

        timelinePreviousMonth() {
            this.$scope.today = moment(this.$scope.today).subtract(1, 'months').toDate();
            this.$rootScope.$broadcast('timeline-previous-month');
        }

        setMapBounds() {
            this.$rootScope.$broadcast('update_locations_map_bounds');
        }

    }

    angular.module("app").controller("projectSummaryPrintController", ProjectSummaryPrintController);
}