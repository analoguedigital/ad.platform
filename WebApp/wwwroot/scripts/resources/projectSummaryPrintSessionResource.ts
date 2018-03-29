module App.Resources {
    "use strict";

    export interface IProjectSummaryPrintSessionResource extends ng.resource.IResourceClass<Models.IProjectSummaryPrintSession> {
        update(relationship: Models.IProjectSummaryPrintSession): Models.IProjectSummaryPrintSession;
    }

    ProjectSummaryPrintSessionResource.$inject = ["$resource"];
    export function ProjectSummaryPrintSessionResource($resource: ng.resource.IResourceService): IProjectSummaryPrintSessionResource {
        return <IProjectSummaryPrintSessionResource>$resource('/api/projectSummaryPrintSession/:id', { id: '@id' },
            {
                'update': { method: 'PUT' }
            });
    }

    angular.module("app").factory("projectSummaryPrintSessionResource", ProjectSummaryPrintSessionResource);
}