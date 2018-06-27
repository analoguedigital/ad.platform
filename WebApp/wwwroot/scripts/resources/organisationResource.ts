module App.Resources {
    "use strict";

    export interface IOrganisationResource extends ng.resource.IResourceClass<Models.IOrganisation> {
        getList(): Models.IOrganisation[];
        update(user: Models.IOrganisation, success: Function, error?: Function): Models.IOrganisation;
        assign(params: Object, success: Function, error?: Function): Models.IOrganisation;
        revoke(params: Object, success: Function, error?: Function);
    }
    
    OrganisationResource.$inject = ["$resource"];
    export function OrganisationResource($resource: ng.resource.IResourceService): IOrganisationResource {
        return <IOrganisationResource>$resource('/api/organisations/:id', { id: '@id' }, {
            'getList': { method: 'GET', url: '/api/organisations/getlist', isArray: true },
            'update': { method: 'PUT' },
            'assign': { method: 'POST', url: '/api/organisations/:id/assign', params: { id: '@id' } },
            'revoke': { method: 'DELETE', url: '/api/organisations/:id/revoke/:userId', params: { id: '@id', userId: '@userId' } }
        });
    }

    angular.module("app").factory("organisationResource", OrganisationResource);
}