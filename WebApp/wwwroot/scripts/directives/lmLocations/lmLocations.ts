declare var google: any;

module App {
    "use strict";

    interface IlmLocations extends ng.IDirective {

    }

    interface IlmLocationsScope extends ng.IScope {
        id: string;
        map: any;
        locations: any[];
        markers: any[];
    }

    interface IlmLocationsAttributes extends ng.IAttributes {

    }

    function lmLocations($injector: ng.auto.IInjectorService): IlmLocations {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'scripts/directives/lmLocations/template.html',
            transclude: false,
            link: link,
            scope: {
                id: '@',
                locations: '='
            }
        };

        function link(scope: IlmLocationsScope,
            element: ng.IAugmentedJQuery,
            attrs: IlmLocationsAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            scope.$watch('locations', (newValue: any[]) => {
                scope.locations = newValue;
            });

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

            // generate markers
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

    angular.module("app").directive("lmLocations", lmLocations);
}