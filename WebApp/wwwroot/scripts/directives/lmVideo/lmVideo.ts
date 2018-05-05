module App {
    "use strict";

    interface IlmVideo extends ng.IDirective {
    }

    interface IlmVideoScope extends ng.IScope {
        code: string;
        url: string;
    }

    interface IlmVideoAttributes extends ng.IAttributes {

    }

    lmVideo.$inject = ['$sce'];
    function lmVideo($sce: ng.ISCEService): IlmVideo {
        return {
            restrict: "A",
            scope: {
                code: '='
            },
            replace: true,
            template: '<video ng-src="{{url}}" controls></video>',
            link: link,
        };


        function link(scope: IlmVideoScope,
            element: ng.IAugmentedJQuery,
            attrs: IlmVideoAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            scope.$watch('code', (newVal, oldVal) => {
                if (newVal !== undefined) {
                    scope.url = $sce.trustAsResourceUrl("/api/downloads/" + newVal);
                }
            });
        }
    }

    angular.module("app").directive("lmVideo", lmVideo);
}