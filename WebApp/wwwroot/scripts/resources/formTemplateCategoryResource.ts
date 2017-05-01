
module App.Resources {
    "use strict";

    export interface IFormTemplateCategoryResource extends ng.resource.IResourceClass<Models.IFormTemplateCategory> {
        update(group: Models.IFormTemplateCategory, success: Function, error?: Function): Models.IFormTemplateCategory;
    }

    FormTemplateCategoryResource.$inject = ["$resource"];
    export function FormTemplateCategoryResource($resource: ng.resource.IResourceService): IFormTemplateCategoryResource {

        return <IMetricGroupResource>$resource('/api/formtemplatecategories/:id',
            { id: '@id' },
            {
                'update': { method: 'PUT' }
            });
    }

    angular.module("app").factory("formTemplateCategoryResource", FormTemplateCategoryResource);

}