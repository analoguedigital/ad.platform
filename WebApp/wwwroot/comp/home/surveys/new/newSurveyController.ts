declare var google: any;
module App {
    "use strict";

    interface GeocodeInformation {
        latitude: number;
        longitude: number;
        accuracy: number;
        address: string;
        formattedAddress: string;
        hasLocation: boolean;
    }

    export interface INewSurveyScope extends ng.IScope {
        ctrl: INewSurveyController;
        geocoding: GeocodeInformation;
        geocoderWorking: boolean;
        geolocationWorking: boolean;

        isInsertMode: boolean;
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

        static $inject: string[] = ["$scope", "$q", "$rootScope", "$state", "$stateParams", "$timeout",
            "toastr", "surveyResource", "formTemplate", "project", "survey", "localStorageService", "userContextService"];

        constructor(
            private $scope: INewSurveyScope,
            private $q: ng.IQService,
            private $rootScope: ng.IRootScopeService,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $timeout: ng.ITimeoutService,
            private toastr: any,
            private surveyResource: Resources.ISurveyResource,
            public formTemplate: Models.IFormTemplate,
            public project: Models.IProject,
            public survey: Models.ISurvey,
            public localStorageService: ng.local.storage.ILocalStorageService,
            public userContextService: App.Services.IUserContextService) {

            if (survey == null) {
                this.survey = <Models.ISurvey>{};
                this.survey.serial = null;
                this.survey.formValues = [];
                this.survey.surveyDate = new Date();
                this.survey.formTemplateId = formTemplate.id;
                this.survey.projectId = project.id;

                //TODO: should not be based on the formtemplate
            }

            $scope.geocoding = {
                latitude: undefined,
                longitude: undefined,
                accuracy: 0,
                address: '',
                formattedAddress: '',
                hasLocation: false
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

            var prevState = this.$rootScope.previousState;
            var prevParams = this.$rootScope.previousStateParams;

            if (prevState && prevState.name.length && prevParams) {
                this.localStorageService.set('survey.prevState', prevState);
                this.localStorageService.set('survey.prevParams', prevParams);
            }

            var surveyId = this.$stateParams["surveyId"];
            this.$scope.isInsertMode = surveyId === undefined;

            // check for advice responses
            if (this.formTemplate.discriminator === 1 && !this.$scope.isInsertMode && this.survey.isRead === false) {
                var orgUser = this.userContextService.current.orgUser;
                if (orgUser !== null && this.survey.projectId === orgUser.currentProjectId) {
                    this.surveyResource.markAsRead({ id: this.survey.id }, (res) => {
                        console.log(res);
                    }, (err) => {
                        console.error(err);
                    });
                }
            }

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
                    let markerTitle = '#' + this.survey.serial;
                    if (this.survey.description.length) {
                        markerTitle += ' - ' + this.survey.description;
                    }

                    this.locations.push({
                        center: { latitude: pos.latitude, longitude: pos.longitude },
                        zoom: 14,
                        options: {
                            scrollwheel: false,
                            mapTypeControl: true,
                            streetViewControl: false
                        },
                        marker: {
                            id: index + 1,
                            coords: { latitude: pos.latitude, longitude: pos.longitude },
                            options: { draggable: false, title: markerTitle },
                            events: {
                                click: (marker, eventName, args) => {
                                    let position = marker.getPosition();
                                    let lat = position.lat();
                                    let lon = position.lng();

                                    let infoWindow = new google.maps.InfoWindow;

                                    let popupContent;

                                    if (pos.address && pos.address.length) {
                                        popupContent = '<p style="margin-bottom: 0"><b>Serial:</b> #' + + this.survey.serial + '<br>' +
                                            '<b>Date:</b> ' + moment(this.survey.surveyDate).format('ddd, MMM Do YYYY') +
                                            '<br><br>' + pos.address + '</p>';
                                    } else {
                                        popupContent = '<p style="margin-bottom: 0"><b>Serial:</b> #' + + this.survey.serial + '<br>' +
                                            '<b>Date:</b> ' + moment(this.survey.surveyDate).format('ddd, MMM Do YYYY') + '</p>';
                                    }

                                    infoWindow.setContent(popupContent);
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

            if (prevState && prevState.name.length && prevParams) {
                this.$state.go(prevState.name, prevParams);
            } else {
                var lsPrevState: any = this.localStorageService.get('survey.prevState');
                var lsPrevParams: any = this.localStorageService.get('survey.prevParams');
                if (lsPrevState && lsPrevState.name.length && lsPrevParams) {
                    this.$state.go(lsPrevState.name, lsPrevParams);
                }
            }
        }

        getAddress() {
            var self = this;

            var geocoder = new google.maps.Geocoder();
            var address = this.$scope.geocoding.address;

            if (address.length < 1) {
                this.toastr.error('Please enter an address first');
                return;
            }

            this.$scope.geocoderWorking = true;
            geocoder.geocode({ 'address': address }, function (result, status) {
                self.$scope.geocoderWorking = false;

                if (status == google.maps.GeocoderStatus.OK) {
                    var formattedAddress = result[0].formatted_address;
                    var lat = result[0].geometry.location.lat();
                    var lng = result[0].geometry.location.lng();

                    self.$scope.$apply(function () {
                        self.$scope.geocoding.latitude = lat;
                        self.$scope.geocoding.longitude = lng;
                        self.$scope.geocoding.address = formattedAddress;
                        self.$scope.geocoding.formattedAddress = formattedAddress;
                        self.$scope.geocoding.hasLocation = true;
                    });

                    self.getMapPreview(lat, lng, formattedAddress);
                } else {
                    self.toastr.error('Something went wrong, could not fetch location');
                }
            });
        }

        getMyLocation() {
            var self = this;

            if (navigator.geolocation) {
                this.$scope.geolocationWorking = true;
                navigator.geolocation.getCurrentPosition((position) => {
                    var latitude = position.coords.latitude;
                    var longitude = position.coords.longitude;
                    var accuracy = position.coords.accuracy;

                    var geocoder = new google.maps.Geocoder();
                    var latLng = new google.maps.LatLng(latitude, longitude);

                    geocoder.geocode({ 'latLng': latLng }, function (res, status) {
                        self.$scope.geolocationWorking = false;

                        if (status == google.maps.GeocoderStatus.OK) {
                            if (res[0]) {
                                var formattedAddress = res[0].formatted_address;

                                self.$scope.$apply(function () {
                                    self.$scope.geocoding.latitude = latitude;
                                    self.$scope.geocoding.longitude = longitude;
                                    self.$scope.geocoding.accuracy = accuracy;
                                    self.$scope.geocoding.address = formattedAddress;
                                    self.$scope.geocoding.formattedAddress = formattedAddress;
                                    self.$scope.geocoding.hasLocation = true;
                                });

                                self.getMapPreview(latitude, longitude, formattedAddress);
                            } else {
                                console.warn('address not found!');
                                self.toastr.error('Address not found!');
                            }
                        } else {
                            self.toastr.error('Something went wrong, address not found');
                        }
                    });
                }, (err: PositionError) => {
                    console.error(err);
                    this.$scope.geolocationWorking = false;

                    if (err.PERMISSION_DENIED) {
                        this.toastr.error('Permission denied by user');
                    } else if (err.POSITION_UNAVAILABLE) {
                        this.toastr.error('Position not available');
                    }
                }, { enableHighAccuracy: true });
            } else {
                console.warn('navigator.geolocation is not available');
                this.toastr.error('Your browser does not support geolocation');
            }
        }

        getMapPreview(latitude: number, longitude: number, address: string) {
            var self = this;

            var latlng = new google.maps.LatLng(latitude, longitude);

            var mapOptions = {
                zoom: 14,
                center: latlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                mapTypeControl: true,
                streetViewControl: false,
                scrollwheel: false
            };

            var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);

            var marker = new google.maps.Marker({
                position: latlng,
                map: map,
                draggable: true,
                title: "location: " + address
            });

            google.maps.event.addListener(marker, 'dragend', function (e) {
                var lat = e.latLng.lat();
                var lng = e.latLng.lng();

                var geocoder = new google.maps.Geocoder();
                var latLng = new google.maps.LatLng(lat, lng);

                self.$scope.geocoderWorking = true;
                geocoder.geocode({ 'latLng': latLng }, function (res, status) {
                    self.$scope.geocoderWorking = false;
                    console.log('result', res);
                    console.log('status', status);

                    if (status == google.maps.GeocoderStatus.OK) {
                        if (res[0]) {
                            var formattedAddress = res[0].formatted_address;

                            self.$scope.$apply(function () {
                                self.$scope.geocoding.latitude = lat;
                                self.$scope.geocoding.longitude = lng;
                                self.$scope.geocoding.address = formattedAddress;
                                self.$scope.geocoding.formattedAddress = formattedAddress;
                            });
                        } else {
                            console.warn('address not found!');
                            self.toastr.error('Address not found!');
                        }
                    } else {
                        self.toastr.error('Something went wrong, address not found');
                    }
                });
            });

            google.maps.event.addListener(marker, 'click', function (e) {
                var infoWindow = new google.maps.InfoWindow();
                infoWindow.setContent(address);
                infoWindow.open(map, marker);
            });
        }

        clearAddress() {
            this.$scope.geocoding = {
                accuracy: 0,
                latitude: undefined,
                longitude: undefined,
                address: '',
                formattedAddress: '',
                hasLocation: false
            };
        }

        setSurveyLocation() {
            var self = this;
            var deferred = this.$q.defer();         

            if (this.$scope.geocoding.hasLocation) {
                this.survey.locations = [];
                this.survey.locations.push({
                    latitude: this.$scope.geocoding.latitude,
                    longitude: this.$scope.geocoding.longitude,
                    accuracy: this.$scope.geocoding.accuracy,
                    address: this.$scope.geocoding.formattedAddress,
                    event: 'Submission',
                    error: ''
                });

                deferred.resolve();
            } else {
                deferred.resolve();
            }

            return deferred.promise;
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                this.toastr.error('Validation failed, please try again');
                return;
            }

            var prevState = this.$rootScope.previousState;
            var prevParams = this.$rootScope.previousStateParams;

            this.setSurveyLocation().then(() => {
                if (this.survey.id == null) {
                    this.surveyResource.save(this.survey).$promise
                        .then(
                        () => {
                            if (prevState.name.length) {
                                this.$state.go(prevState.name, prevParams);
                            } else {
                                this.$state.go('home.surveys.list.summary', { projectId: this.project.id });
                            }
                        },
                        (err) => {
                            console.error(err);
                            if (err.status == 400) {
                                this.toastr.error(err.data.message);
                            } else {
                                this.toastr.error('An error has occured, sorry');
                            }
                        });
                }
                else {
                    this.surveyResource.update(this.survey,
                        () => {
                            if (prevState.name) {
                                this.$state.go(prevState.name, prevParams);
                            } else {
                                this.$state.go('home.surveys.list.summary', { projectId: this.survey.projectId });
                            }
                        },
                        (err) => {
                            console.log(err);
                            this.toastr.error(err.data);
                        });
                }

            }, (err) => {
                console.error('SET LOCATION ERROR', err);
            });
        }
    }

    angular.module("app").controller("newSurveyController", NewSurveyController);
}