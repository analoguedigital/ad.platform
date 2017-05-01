
module App.Resources {
    "use strict";

    export interface ISurveyResource extends ng.resource.IResourceClass<Models.ISurvey> {
        update(survey: Models.ISurvey, success: Function, error?: Function): Models.ISurvey;
    }

    SurveyResource.$inject = ["$resource"];
    export function SurveyResource($resource: ng.resource.IResourceService): ISurveyResource {

        return <ISurveyResource>$resource('/api/surveys/:id', { id: '@id' }, { 
            'get': {
                interceptor: {
                    response: (response) => {
                        _.each(response.data.formValues, (formValue: any) => {
                            if (formValue.dateValue !== undefined && formValue.dateValue !== null && formValue.dateValue !== '') {
                                formValue.dateValue = new Date(formValue.dateValue);
                            }
                        });
                        return response.data;
                    }
                }
            },
            'update': { method: 'PUT' }
        });

    }


    angular.module("app").factory("surveyResource", SurveyResource);
}