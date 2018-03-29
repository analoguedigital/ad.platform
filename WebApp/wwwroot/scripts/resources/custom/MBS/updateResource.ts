module App.Resources {
    "use strict";

    export interface IUpdateResource extends ng.resource.IResourceClass<Models.MBS.IUpdate> { }

    UpdateResource.$inject = ["$resource"];
    export function UpdateResource($resource: ng.resource.IResourceService): IUpdateResource {
        return <IUpdateResource>$resource('/api/mbs/updates', { });
    }

    angular.module("app").factory("updateResource", UpdateResource);
}