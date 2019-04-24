module App.Resources {
    "use strict";

    export interface ISurveyResource extends ng.resource.IResourceClass<Models.ISurvey> {
        update(survey: Models.ISurvey, success: Function, error?: Function): Models.ISurvey;
        search(model: Models.SearchDTO, success: Function, error?: Function): Array<Models.ISurvey>;
        markAsRead(params: Object, success: Function, error?: Function): void;
    }

    SurveyResource.$inject = ["$resource"];
    export function SurveyResource($resource: ng.resource.IResourceService): ISurveyResource {

        return <ISurveyResource>$resource('/api/surveys/:id', { id: '@id' }, {
            'get': { method: 'GET' },
            'update': { method: 'PUT' },
            'search': { method: 'POST', url: '/api/surveys/search', isArray: true },
            'markAsRead': { method: 'POST', url: '/api/surveys/:id/mark-as-read' }
        });
    }

    angular.module("app").factory("surveyResource", SurveyResource);
}