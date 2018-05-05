module App {
    "use strict";

    interface IlmAudio extends ng.IDirective {
    }

    interface IlmAudioScope extends ng.IScope {
        code: string;
        url: string;
    }

    interface IlmAudioAttributes extends ng.IAttributes {
        
    }

    lmAudio.$inject = ['$sce'];
    function lmAudio($sce: ng.ISCEService): IlmAudio {
        return {
            restrict: "A",
            scope: {
                code: '='
            },
            replace: true,
            template: '<audio ng-src="{{url}}" controls></audio>',
            link: link,
        };


        function link(scope: IlmAudioScope,
            element: ng.IAugmentedJQuery,
            attrs: IlmAudioAttributes,
            ctrl: any,
            transclude: ng.ITranscludeFunction) {

            scope.$watch('code', (newVal, oldVal) => {
                if (newVal !== undefined) {
                    scope.url = $sce.trustAsResourceUrl("/api/downloads/" + newVal);
                }
            });
        }
    }

    angular.module("app").directive("lmAudio", lmAudio);
}