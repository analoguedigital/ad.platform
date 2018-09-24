module App.Resources {
    "use strict";

    export interface IEmailRecipientResource extends ng.resource.IResourceClass<Models.IEmailRecipient> {
        assign(params: Object, success: Function, error?: Function);
        unassign(params: Object, success: Function, error?: Function);
    }

    EmailRecipientResource.$inject = ["$resource"];
    export function EmailRecipientResource($resource: ng.resource.IResourceService): IProjectResource {
        return <IProjectResource>$resource('/api/email-recipients/:id', { id: '@id' }, {
            'assign': { method: 'POST', url: '/api/email-recipients/assign/:userId/:flag', params: { userId: '@userId', flag: '@flag' } },
            'unassign': { method: 'DELETE', url: '/api/email-recipients/assign/:userId/:flag', params: { userId: '@userId', flag: '@flag' } },            
        });
    }

    angular.module("app").factory("emailRecipientResource", EmailRecipientResource);
}