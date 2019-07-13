module App {
    "use strict";

    interface ISlideshowControllerScope extends ng.IScope {
        impactRate: number;
        surveyDescription: string;
        attachments: Models.IAttachment[];
        address: string;
        location: { latitude: number, longitude: number };

        rateSliderOpts: any;
    }

    interface ISlideshowController {
        activate: () => void;
        close: () => void;
    }

    class SlideshowController implements ISlideshowController {
        recordCount: number;
        currentIndex: number;

        survey: Models.ISurvey;

        static $inject: string[] = ["$scope", "$timeout", "$compile", "$uibModalInstance", "toastr", "surveys", "formTemplates"];
        constructor(
            private $scope: ISlideshowControllerScope,
            private $timeout: ng.ITimeoutService,
            private $compile: ng.ICompileService,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            public toastr: any,
            public surveys: Models.ISurvey[],
            public formTemplates: Models.IFormTemplate[]) {
            this.activate();
        }

        activate() {
            this.recordCount = this.surveys.length;
            this.currentIndex = 1;

            this.survey = this.surveys[this.currentIndex - 1];
            this.updateSurvey();

            this.$timeout(() => {
                this.$scope.$broadcast('rzSliderForceRender');
            }, 500);
        }

        pinSymbol(color) {
            return {
                path: 'M 0,0 C -2,-20 -10,-22 -10,-30 A 10,10 0 1,1 10,-30 C 10,-22 2,-20 0,0 z M -2,-30 a 2,2 0 1,1 4,0 2,2 0 1,1 -4,0',
                fillColor: color,
                fillOpacity: 1,
                strokeColor: '#333',
                strokeWeight: 2,
                scale: 1,
            };
        }

        updateSurvey() {
            var self = this;

            this.$scope.attachments = [];
            this.$scope.location = { latitude: undefined, longitude: undefined };

            _.forEach(this.survey.formValues, (fv) => {
                if (fv.numericValue !== null) {
                    this.$scope.impactRate = fv.numericValue;
                }

                if (fv.textValue !== null && fv.textValue.length) {
                    this.$scope.surveyDescription = fv.textValue;
                }

                if (fv.attachments.length) {
                    this.$scope.attachments = fv.attachments;
                }
            });

            var rateSliderSteps = [
                { value: 0, legend: 'None' },
                { value: 1, legend: 'Slight' },
                { value: 2, legend: 'Moderate' },
                { value: 3, legend: 'High' }
            ];

            this.$scope.rateSliderOpts = {
                readOnly: true,
                showTicks: true,
                showTicksValues: true,
                stepsArray: rateSliderSteps
            };

            if (this.survey.locations.length) {
                var position = this.survey.locations[0];
                this.$scope.address = position.address;
                this.$scope.location.latitude = position.latitude;
                this.$scope.location.longitude = position.longitude;

                this.$scope.map = {
                    center: { latitude: position.latitude, longitude: position.longitude },
                    zoom: 14,
                    options: {
                        scrollwheel: false,
                        mapTypeId: google.maps.MapTypeId.ROADMAP,
                        mapTypeControl: true,
                        streetViewControl: false
                    }
                };

                let pinColor = "ffffff";
                let template = _.find(self.formTemplates, (t) => { return t.id == self.survey.formTemplateId; });
                if (template) pinColor = template.colour;

                let pinImage = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|" + pinColor,
                    new google.maps.Size(21, 34),
                    new google.maps.Point(0, 0),
                    new google.maps.Point(10, 34));
                let pinShadow = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_shadow",
                    new google.maps.Size(40, 37),
                    new google.maps.Point(0, 0),
                    new google.maps.Point(12, 35));

                this.$scope.marker = {
                    id: self.survey.id,
                    coords: { latitude: position.latitude, longitude: position.longitude },
                    options: {
                        draggable: false,
                        //icon: pinImage,
                        icon: this.pinSymbol(pinColor),
                        shadow: pinShadow,
                        title: '#' + self.survey.serial,
                        event: position.event
                    },
                    events: {
                        click: function (marker, eventName, args) {
                            var lat = marker.getPosition().lat();
                            var lon = marker.getPosition().lng();

                            var surveyDate = moment(self.survey.surveyDate).format('ddd, MMM Do YYYY')

                            let infoWindow = new google.maps.InfoWindow;

                            let surveyLink = `<p><a target='_blank' ui-sref='home.surveys.view({surveyId: "${self.survey.id}"})'><i class='fa fa-arrow-right'> go to record</a></p>`;
                            let compiledLink = self.$compile(surveyLink)(self.$scope);

                            let popupContent;
                            if (self.$scope.address && self.$scope.address.length) {
                                popupContent = '<p style="margin-bottom: 0"><b>Serial:</b> #' + + self.survey.serial + '<br>' +
                                    '<b>Date:</b> ' + surveyDate + '<br><br>' + self.$scope.address + '</p>' +
                                    '<p style="margin-top: 5px; padding-top: 5px; margin-bottom: 0; border-top: 1px solid #ddd">' + compiledLink[0].innerHTML + '</p>';
                            } else {
                                popupContent = '<p style="margin-bottom: 0"><b>Serial:</b> #' + self.survey.serial + '<br>' +
                                    '<b>Date:</b> ' + surveyDate + '</p>' +
                                    '<p style="margin-top: 5px; padding-top: 5px; margin-bottom: 0; border-top: 1px solid #ddd">' + compiledLink[0].innerHTML + '</p>';
                            }

                            infoWindow.setContent(popupContent);
                            infoWindow.open(self.$scope.map, marker);
                        }
                    }
                };
            }
        }

        goFirst() {
            this.currentIndex = 1;
            this.survey = this.surveys[this.currentIndex - 1];
            this.updateSurvey();
        }

        goNext() {
            if (this.currentIndex + 1 > this.surveys.length) return;

            this.currentIndex += 1;
            this.survey = this.surveys[this.currentIndex - 1];
            this.updateSurvey();
        }

        goPrev() {
            if (this.currentIndex - 1 < 1) return;

            this.currentIndex -= 1;
            this.survey = this.surveys[this.currentIndex - 1];
            this.updateSurvey();
        }

        goLast() {
            this.currentIndex = this.surveys.length;
            this.survey = this.surveys[this.currentIndex - 1];
            this.updateSurvey();
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };

        swipeLeft() {
            var elem = angular.element('#survey-container');
            $(elem).animate({ left: '-=50' }, 150);
            $(elem).animate({ left: '+=50' }, 250);

            this.goNext();
        }

        swipeRight() {
            var elem = angular.element('#survey-container');
            $(elem).animate({ left: '+=50' }, 150);
            $(elem).animate({ left: '-=50' }, 250);

            this.goPrev();
        }
    }

    angular.module("app").controller("slideshowController", SlideshowController);
}