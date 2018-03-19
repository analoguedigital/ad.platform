module App.Resources {
    "use strict";

    export interface IOrgUserTypeResource extends ng.resource.IResourceClass<Models.IOrgUserType> { }

    OrgUserTypeResource.$inject = ["$resource"];
    export function OrgUserTypeResource($resource: ng.resource.IResourceService): IOrgUserTypeResource {
        return <IOrgUserTypeResource>$resource('/api/orgUserTypes');
    }

    angular.module("app").factory("orgUserTypeResource", OrgUserTypeResource);
}