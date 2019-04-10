declare var google: any;

module App {
    "use strict";

    interface IlmLocations extends ng.IDirective {

    }

    interface IlmLocationsScope extends ng.IScope {
        id: string;
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        map: any;
        locations: Models.ISurveyLocation[];
        markers: any[];
        locationCount: number;
        center: any;
        zoomLevel: number;
        mapType: string;
    }

    interface IlmLocationsAttributes extends ng.IAttributes {

    }

    lmLocations.$inject = ['$injector', '$rootScope', '$timeout', 'moment', '$compile', 'localStorageService'];
    function lmLocations(
        $injector: ng.auto.IInjectorService,
        $rootScope: ng.IRootScopeService,
        $timeout: ng.ITimeoutService,
        moment: any,
        $compile: ng.ICompileService,
        localStorageService: ng.local.storage.ILocalStorageService): IlmLocations {

        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'scripts/directives/lmLocations/template.html',
            transclude: false,
            link: link,
            scope: {
                id: '@',
                formTemplates: '=',
                surveys: '=',
                locationCount: '=',
                center: '=',
                zoomLevel: '=',
                mapType: '='
            }
        };

        function link(scope: IlmLocationsScope,
            element: ng.IAugmentedJQuery,
            attrs: IlmLocationsAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            scope.$watchGroup(['formTemplates', 'surveys'], (threads, surveys) => {
                extractLocations();
                setGoogleMap();
                generateMarkers();
            });

            function extractLocations() {
                scope.locations = [];
                angular.forEach(scope.surveys, (survey) => {
                    angular.forEach(survey.locations, (loc) => {
                        let template = _.find(scope.formTemplates, (template) => { return template.id == survey.formTemplateId; });
                        if (template) {
                            let location: Models.ISurveyLocation = {
                                // position data
                                accuracy: loc.accuracy,
                                error: loc.error,
                                event: loc.event,
                                latitude: loc.latitude,
                                longitude: loc.longitude,
                                address: loc.address,
                                // survey data
                                id: survey.id,
                                color: template.colour,
                                description: survey.description,
                                serial: survey.serial,
                                date: moment(survey.surveyDate).format('ddd, MMM Do YYYY')
                            };

                            scope.locations.push(location);
                        }
                    });
                });

                scope.locationCount = scope.locations.length;
            }

            function setGoogleMap() {
                // default center to london
                //let center = { latitude: 51.518732, longitude: -0.127029 };

                let center = { latitude: undefined, longitude: undefined };

                if (scope.center && scope.center.latitude && scope.center.longitude) {
                    center = scope.center;
                }

                let _mapType;
                if (scope.mapType && scope.mapType.length) {
                    if (scope.mapType == 'roadmap')
                        _mapType = google.maps.MapTypeId.ROADMAP;
                    else if (scope.mapType == 'hybrid')
                        _mapType = google.maps.MapTypeId.HYBRID;
                } else {
                    _mapType = google.maps.MapTypeId.ROADMAP;
                }

                scope.map = {
                    center: center,
                    zoom: 14,
                    options: {
                        scrollwheel: false,
                        mapTypeId: _mapType,
                        mapTypeControl: true,
                        streetViewControl: false
                    },
                    events: {
                        'zoom_changed': function (map, eventName, args) {
                            scope.zoomLevel = map.getZoom();
                            localStorageService.set('export_map_zoom_level', scope.zoomLevel);
                        },
                        'maptypeid_changed': function (map, eventName, args) {
                            scope.mapType = map.getMapTypeId();
                            localStorageService.set('export_map_type', scope.mapType);
                        },
                        'center_changed': function (map, eventName, args) {
                            localStorageService.set('export_map_center', scope.center);
                        }
                    }
                };

                scope.zoomLevel = scope.map.zoom;
                scope.center = scope.map.center;
                scope.mapType = scope.map.options.mapTypeId;
            }

            function pinSymbol(color) {
                return {
                    path: 'M 0,0 C -2,-20 -10,-22 -10,-30 A 10,10 0 1,1 10,-30 C 10,-22 2,-20 0,0 z M -2,-30 a 2,2 0 1,1 4,0 2,2 0 1,1 -4,0',
                    fillColor: color,
                    fillOpacity: 1,
                    strokeColor: '#333',
                    strokeWeight: 2,
                    scale: 1,
                };
            }

            function generateMarkers() {
                // gmaps v3 custom icons:
                // https://stackoverflow.com/questions/7095574/google-maps-api-3-custom-marker-color-for-default-dot-marker/7686977

                scope.markers = [];
                angular.forEach(scope.locations, (loc, index) => {
                    let pinColor = "ffffff";
                    if (loc.color.length)
                        pinColor = loc.color.substr(1, loc.color.length - 1);

                    try {
                        let pinImage = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|" + pinColor,
                            new google.maps.Size(21, 34),
                            new google.maps.Point(0, 0),
                            new google.maps.Point(10, 34));
                        let pinShadow = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_shadow",
                            new google.maps.Size(40, 37),
                            new google.maps.Point(0, 0),
                            new google.maps.Point(12, 35));

                        // one option is to show serials and descriptions:
                        // #1234 - survey description
                        let markerTitle = '#' + loc.serial;
                        if (loc.description.length)
                            markerTitle += ' - ' + loc.description;

                        let marker = {
                            id: index,
                            coords: {
                                latitude: loc.latitude,
                                longitude: loc.longitude
                            },
                            options: {
                                draggable: false,
                                //icon: pinImage,
                                icon: pinSymbol(loc.color),
                                shadow: pinShadow,
                                title: markerTitle,
                                event: loc.event
                            },
                            events: {
                                click: function (marker, eventName, args) {
                                    var lat = marker.getPosition().lat();
                                    var lon = marker.getPosition().lng();

                                    let infoWindow = new google.maps.InfoWindow;
                                    //if (marker.title.length)    // if survey has description
                                    //    infoWindow.setContent(marker.title);
                                    //else
                                    //    infoWindow.setContent(marker.event);

                                    let surveyLink = `<p><a target='_blank' ui-sref='home.surveys.view({surveyId: "${loc.id}"})'><i class='fa fa-arrow-right'> go to record</a></p>`;
                                    let compiledLink = $compile(surveyLink)(scope);

                                    let popupContent;
                                    if (loc.address && loc.address.length) {
                                        popupContent = '<p style="margin-bottom: 0"><b>Serial:</b> #' + + loc.serial + '<br>' +
                                            '<b>Date:</b> ' + loc.date + '<br><br>' + loc.address + '</p>' +
                                            '<p style="margin-top: 5px; padding-top: 5px; margin-bottom: 0; border-top: 1px solid #ddd">' + compiledLink[0].innerHTML + '</p>';
                                    } else {
                                        popupContent = '<p style="margin-bottom: 0"><b>Serial:</b> #' + + loc.serial + '<br>' +
                                            '<b>Date:</b> ' + loc.date + '</p>' +
                                            '<p style="margin-top: 5px; padding-top: 5px; margin-bottom: 0; border-top: 1px solid #ddd">' + compiledLink[0].innerHTML + '</p>';
                                    }

                                    infoWindow.setContent(popupContent);
                                    infoWindow.open(scope.map, marker);
                                }
                            }
                        };

                        scope.markers.push(marker);
                    } catch (e) {
                        console.error(e);
                    }
                });
            }

            $rootScope.$on('update_locations_map_bounds', (event, args: any) => {
                $timeout(() => {
                    scope.$apply(() => {
                        if (scope.markers.length) {
                            var bounds = new google.maps.LatLngBounds();
                            _.forEach(scope.markers, (marker) => {
                                var position = new google.maps.LatLng(marker.coords.latitude, marker.coords.longitude);
                                bounds.extend(position);
                            });

                            var neLat = bounds.getNorthEast().lat();
                            var neLng = bounds.getNorthEast().lng();

                            var swLat = bounds.getSouthWest().lat();
                            var swLng = bounds.getSouthWest().lng();

                            scope.bd = {
                                northeast: {
                                    latitude: neLat,
                                    longitude: neLng
                                },
                                southwest: {
                                    latitude: swLat,
                                    longitude: swLng
                                }
                            };
                        }
                    });
                }, 1000);
            });

            $rootScope.$on('update_locations_map', (event, args: any) => {
                if (args.zoomLevel == undefined || args.center == undefined || args.center.latitude == undefined || args.center.longitude == undefined)
                    return;

                if (args.zoomLevel == 0 || isNaN(args.center.latitude) || isNaN(args.center.longitude)) {
                    return;
                }

                $timeout(() => {
                    scope.$apply(() => {
                        let _mapType;
                        if (args.mapType && args.mapType.length) {
                            if (args.mapType == 'roadmap')
                                _mapType = google.maps.MapTypeId.ROADMAP;
                            else if (args.mapType == 'hybrid')
                                _mapType = google.maps.MapTypeId.HYBRID;
                        } else {
                            _mapType = google.maps.MapTypeId.ROADMAP;
                        }

                        scope.map = {
                            center: args.center,
                            zoom: args.zoomLevel,
                            options: {
                                scrollwheel: false,
                                mapTypeControl: true,
                                streetViewControl: false,
                                mapTypeId: _mapType,
                            },
                            events: {
                                'zoom_changed': function (map, eventName, args) {
                                    //scope.$apply(() => {
                                        
                                    //});

                                    scope.zoomLevel = map.getZoom();
                                    localStorageService.set('export_map_zoom_level', scope.zoomLevel);
                                },
                                'maptypeid_changed': function (map, eventName, args) {
                                    //scope.$apply(() => {
                                        
                                    //});

                                    scope.mapType = map.getMapTypeId();
                                    localStorageService.set('export_map_type', scope.mapType);
                                },
                                'center_changed': function(map, eventName, args) {
                                    localStorageService.set('export_map_center', scope.center);
                                }
                            }
                        };
                    });
                }, 500);
            });
        }
    }

    angular.module("app").directive("lmLocations", lmLocations);
}