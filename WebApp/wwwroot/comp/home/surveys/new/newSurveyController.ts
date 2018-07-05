declare var google: any;
module App {
    "use strict";

    export interface INewSurveyScope extends ng.IScope {
        ctrl: INewSurveyController;
    }

    interface INewSurveyController {
        title: string;
        pages: _.Dictionary<Models.IMetricGroup[]>;
        tabs: any[];
        locations: any[];
        survey: Models.ISurvey;
        activeTabIndex: number;
        activate: () => void;
        addFormValue: (metric, rowDataListItem, rowNumber) => Models.IFormValue;
    }

    class NewSurveyController implements INewSurveyController {
        title: string = "Forms";
        pages: _.Dictionary<Models.IMetricGroup[]>;
        tabs: any[];
        locations: any[];
        //survey: Models.ISurvey;
        activeTabIndex: number = 0;

        static $inject: string[] = ["$scope", "$rootScope", "$state", "$timeout", "toastr", "surveyResource", "formTemplate", "project", "survey"];

        constructor(
            private $scope: INewSurveyScope,
            private $rootScope: ng.IRootScopeService,
            private $state: ng.ui.IStateService,
            private $timeout: ng.ITimeoutService,
            private toastr: any,
            private surveyResource: Resources.ISurveyResource,
            public formTemplate: Models.IFormTemplate,
            public project: Models.IProject,
            public survey: Models.ISurvey) {

            if (survey == null) {
                this.survey = <Models.ISurvey>{};
                this.survey.serial = null;
                this.survey.formValues = [];
                this.survey.surveyDate = new Date();
                this.survey.formTemplateId = formTemplate.id;
                this.survey.projectId = project.id;

                //TODO: should not be based on the formtemplate
            }
            this.activate();
        }

        activate() {
            var pageGroups = _.groupBy(this.formTemplate.metricGroups, (mg) => { return mg.page });
            this.tabs = _.map(Object.keys(pageGroups), (pageNumber) => { return { number: pageNumber, title: "Page " + pageNumber }; });
            this.tabs[0].active = true;

            this.$scope.$on('$viewContentLoaded', () => {
                this.$timeout(() => {
                    this.$scope.$broadcast('rzSliderForceRender');
                }, 500);
            });

            this.getLocations();
        }

        getLocations() {
            let positions = [];
            _.forEach(this.survey.locations, (loc: Models.IPosition) => {
                positions.push(loc);
            });

            this.locations = [];
            if (positions.length) {
                _.forEach(positions, (pos: Models.IPosition, index) => {
                    this.locations.push({
                        center: { latitude: pos.latitude, longitude: pos.longitude },
                        zoom: 10,
                        options: { scrollwheel: false },
                        marker: {
                            id: index + 1,
                            coords: { latitude: pos.latitude, longitude: pos.longitude },
                            options: { draggable: false, title: pos.event },
                            events: {
                                click: (marker, eventName, args) => {
                                    let position = marker.getPosition();
                                    let lat = position.lat();
                                    let lon = position.lng();
                                    console.log(`lat: ${lat} lon: ${lon}`);

                                    let infoWindow = new google.maps.InfoWindow;
                                    infoWindow.setContent(marker.title);
                                    infoWindow.open(this.$scope.map, marker);
                                }
                            }
                        }
                    });
                });
            }
        }

        addFormValue(metric, rowDataListItem, rowNumber) {
            var formValue = <Models.IFormValue>{};
            formValue.textValue = '';
            formValue.metricId = metric.id;
            formValue.rowNumber = rowNumber;
            if (rowDataListItem)
                formValue.rowDataListItemId = rowDataListItem.id;
            this.survey.formValues.push(formValue);
            return formValue;
        };

        next() {
            if (this.activeTabIndex + 1 == this.tabs.length)
                return;

            this.activeTabIndex += 1;
        }

        previous() {
            if (this.activeTabIndex == 0)
                return;

            this.activeTabIndex -= 1;
        }

        previousState() {
            var prevState = this.$rootScope.previousState;
            var prevParams = this.$rootScope.previousStateParams;
            this.$state.go(prevState.name, prevParams);
        }

        submit(form: ng.IFormController) {
            if (form.$invalid)
                return;

            var prevState = this.$rootScope.previousState;
            var prevParams = this.$rootScope.previousStateParams;

            if (this.survey.id == null) {
                this.surveyResource.save(this.survey).$promise
                    .then(
                    () => {
                        //this.$state.go('home.surveys.list.summary', { projectId: this.project.id });
                        this.$state.go(prevState.name, prevParams);
                    },
                    (err) => {
                        console.log(err);
                        this.toastr.error(err.data);
                    });
            }
            else {
                this.surveyResource.update(this.survey,
                    () => {
                        //this.$state.go('home.surveys.list.summary', { projectId: this.survey.projectId });
                        this.$state.go(prevState.name, prevParams);
                    },
                    (err) => {
                        console.log(err);
                        this.toastr.error(err.data);    
                    });
            }

        }
    }

    angular.module("app").controller("newSurveyController", NewSurveyController);
}