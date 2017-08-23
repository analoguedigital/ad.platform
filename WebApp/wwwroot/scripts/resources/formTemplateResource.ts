
module App.Resources {
    "use strict";

    export interface IFormTemplateResource extends angular.resource.IResourceClass<Models.IFormTemplate> {
        update(formTemplate: Models.IFormTemplate, success: Function, error?: Function): Models.IFormTemplate;
        archive(params: Object, success: Function, error?: Function): void;
        publish(params: Object, success: Function, error?: Function): void;
        getFilters(params: Object, success: Function, error?: Function): any[];
    }

    FormTemplateResource.$inject = ["$resource"];
    export function FormTemplateResource($resource: angular.resource.IResourceService): IFormTemplateResource {

        return <IFormTemplateResource>$resource('/api/formtemplates/:id', { id: '@id' }, {
            'update': { method: 'PUT' },
            'archive': { method: 'DELETE', url: '/api/formtemplates/:id/publish', params: { id: '@id' }, },
            'publish': { method: 'PUT', url: '/api/formtemplates/:id/publish', params: { id: '@id' } },
            'getFilters': { method: 'GET', url: '/api/formtemplates/:id/filters', params: { id: '@id' }, isArray: true }
        });

    }


    angular.module("app").factory("formTemplateResource", FormTemplateResource);
}