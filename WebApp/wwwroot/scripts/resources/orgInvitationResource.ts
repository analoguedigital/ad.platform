module App.Resources {
    "use strict";

    export interface IOrgInvitationResource extends ng.resource.IResourceClass<Models.IOrgInvitation> {
        update(invitation: Models.IOrgInvitation, success: Function, error?: Function): Models.IOrgInvitation;
        join(params: Object, success: Function, error?: Function): void;
    }

    OrgInvitationResource.$inject = ["$resource"];
    export function OrgInvitationResource($resource: ng.resource.IResourceService): IOrgInvitationResource {

        return <IOrgInvitationResource>$resource('/api/orginvitations/:id', { id: '@id' }, {
            'update': { method: 'PUT' },
            'join': { method: 'POST', url: '/api/subscriptions/joinorganisation/:token', params: { token: '@token' } }
        });
    }

    angular.module("app").factory("orgInvitationResource", OrgInvitationResource);
}