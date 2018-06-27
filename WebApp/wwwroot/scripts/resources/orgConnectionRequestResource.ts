module App.Resources {
    "use strict";

    export interface IOrgConnectionRequestResource extends ng.resource.IResourceClass<Models.IOrgConnectionRequest> {
        requestToJoin(params: Object, success: Function, error?: Function);
        approve(params: Object, success: Function, error?: Function);
    }

    OrgConnectionRequestResource.$inject = ["$resource"];
    export function OrgConnectionRequestResource($resource: ng.resource.IResourceService): IOrgConnectionRequestResource {
        return <IOrgConnectionRequestResource>$resource('/api/orgConnectionRequests/:id', { id: '@id' }, {
            'requestToJoin': { method: 'POST', url: '/api/orgConnectionRequests/:organisationId', params: { organisationId: '@organisationId' } } ,
            'approve': { method: 'POST', url: '/api/orgConnectionRequests/:id/approve', params: { id: '@id' } }
        });
    }

    angular.module("app").factory("orgConnectionRequestResource", OrgConnectionRequestResource);
}