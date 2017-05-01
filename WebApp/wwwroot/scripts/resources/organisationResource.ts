
module App.Resources {
    "use strict";

    export interface IOrganisationResource extends ng.resource.IResourceClass<Models.IOrganisation> {
        update(user: Models.IOrganisation, success: Function, error?: Function): Models.IOrganisation;
    }

    OrganisationResource.$inject = ["$resource"];
    export function OrganisationResource($resource: ng.resource.IResourceService): IOrganisationResource {

        return <IOrganisationResource>$resource('/api/organisations/:id', { id: '@id' }, {
            'update': { method: 'PUT' }
        });

    }

    angular.module("app").factory("organisationResource", OrganisationResource);
}