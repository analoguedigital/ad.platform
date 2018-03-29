module App {
    "use strict";

    interface IlmZoomGallery extends ng.IDirective { }

    interface IlmZoomGalleryScope extends ng.IScope { }

    lmZoomGallery.$inject = [];
    function lmZoomGallery(): IlmZoomGallery {
        return {
            restrict: "A",
            link: link
        };

        function link(scope: IlmZoomGalleryScope,
            element: ng.IAugmentedJQuery,
            attrs: ng.IAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            $(element).magnificPopup({
                delegate: 'a.mp-link',
                type: 'image',
                closeOnContentClick: false,
                closeBtnInside: false,
                mainClass: 'mfp-with-zoom mfp-img-mobile',
                image: {
                    verticalFit: true
                },
                gallery: {
                    enabled: true
                },
                zoom: {
                    enabled: true,
                    duration: 300, // don't foget to change the duration also in CSS
                    opener: function (element) {
                        return element.find('img');
                    }
                }
            });
        }
    }

    angular.module("app").directive("lmZoomGallery", lmZoomGallery);
}