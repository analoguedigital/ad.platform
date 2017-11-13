
module App.Resources {
    "use strict";

    export interface IDataResource extends ng.resource.IResourceClass<any> {
    }

    DataResource.$inject = ["$resource"];
    export function DataResource($resource: ng.resource.IResourceService): IDataResource {
        return <IDataResource>$resource('/api/projects/:projectId/formTemplates/:formTemplateId/data',
            { projectId: '@projectId', formTemplateId: '@templateID' },
            {  });
    }

    angular.module("app").factory("dataResource", DataResource);
}