module App.Resources {
    "use strict";

    export interface IOrgRequestResource extends ng.resource.IResourceClass<Models.IOrgRequest> {
        sendRequest(params: Object, success: Function, error?: Function);
    }

    OrgRequestResource.$inject = ["$resource"];
    export function OrgRequestResource($resource: ng.resource.IResourceService): IOrgRequestResource {
        return <IOrgRequestResource>$resource('/api/orgRequests/:id', { id: '@id' }, {
            'sendRequest': { method: 'POST', url: '/api/orgRequests/' }
        });
    }

    angular.module("app").factory("orgRequestResource", OrgRequestResource);
}