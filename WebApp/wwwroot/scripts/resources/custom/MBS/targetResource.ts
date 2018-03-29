module App.Resources {
    "use strict";

    export interface ITargetResource extends ng.resource.IResourceClass<Models.MBS.ITarget> { }

    TargetResource.$inject = ["$resource"];
    export function TargetResource($resource: ng.resource.IResourceService): ITargetResource {
        return <ITargetResource>$resource('/api/mbs/students/:projectId/targets', { projectId: "@projectId" });
    }

    angular.module("app").factory("targetResource", TargetResource);
}