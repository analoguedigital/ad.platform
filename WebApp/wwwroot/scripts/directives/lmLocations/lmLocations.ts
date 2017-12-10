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
        locations: ProjectSummaryController.Models.ISurveyLocation[];
        markers: any[];
        locationCount: number;
    }

    interface IlmLocationsAttributes extends ng.IAttributes {

    }

    lmLocations.$inject = ['$injector'];
    function lmLocations($injector: ng.auto.IInjectorService): IlmLocations {
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
                locationCount: '='
            }
        };
        
        function link(scope: IlmLocationsScope,
            element: ng.IAugmentedJQuery,
            attrs: IlmLocationsAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            scope.$watchGroup(['formTemplates', 'surveys'], () => {
                extractLocations();
                setGoogleMap();
                generateMarkers();
            });

            function extractLocations() {
                scope.locations = [];
                angular.forEach(scope.surveys, (survey) => {
                    angular.forEach(survey.locations, (loc) => {
                        let template = _.find(scope.formTemplates, (template) => { return template.id == survey.formTemplateId; });

                        let location: ProjectSummaryController.Models.ISurveyLocation = {
                            accuracy: loc.accuracy,
                            error: loc.error,
                            event: loc.event,
                            latitude: loc.latitude,
                            longitude: loc.longitude,
                            description: survey.description,
                            color: template.colour
                        };

                        scope.locations.push(location);
                    });
                });

                scope.locationCount = scope.locations.length;
            }

            function setGoogleMap() {
                // default center to london
                let center = { latitude: 51.518732, longitude: -0.127029 };
                if (scope.locations.length) {
                    let loc = scope.locations[0];
                    center.latitude = loc.latitude;
                    center.longitude = loc.longitude;
                }

                scope.map = {
                    center: center,
                    zoom: 10,
                    options: { scrollwheel: false }
                };
            }

            function generateMarkers() {
                scope.markers = [];
                angular.forEach(scope.locations, (loc, index) => {
                    let pinColor = "ffffff";
                    if (loc.color.length)
                        pinColor = loc.color.substr(1, loc.color.length - 1);

                    let pinImage = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|" + pinColor,
                        new google.maps.Size(21, 34),
                        new google.maps.Point(0, 0),
                        new google.maps.Point(10, 34));
                    let pinShadow = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_shadow",
                        new google.maps.Size(40, 37),
                        new google.maps.Point(0, 0),
                        new google.maps.Point(12, 35));

                    let marker = {
                        id: index,
                        coords: {
                            latitude: loc.latitude,
                            longitude: loc.longitude
                        },
                        options: {
                            draggable: false,
                            icon: pinImage,
                            shadow: pinShadow,
                            title: loc.description,
                            event: loc.event
                        },
                        events: {
                            click: function (marker, eventName, args) {
                                var lat = marker.getPosition().lat();
                                var lon = marker.getPosition().lng();
                                console.log(`lat: ${lat} lon: ${lon}`);

                                let infoWindow = new google.maps.InfoWindow;
                                if (marker.title.length)
                                    infoWindow.setContent(marker.title);
                                else
                                    infoWindow.setContent(marker.event);

                                infoWindow.open(scope.map, marker);
                            }
                        }
                    };

                    scope.markers.push(marker);
                });
            }
        }
    }

    angular.module("app").directive("lmLocations", lmLocations);
}