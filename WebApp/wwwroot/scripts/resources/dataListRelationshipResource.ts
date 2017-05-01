
module App.Resources {
    "use strict";

    export interface IDataListRelationshipResource extends ng.resource.IResourceClass<Models.IDataListRelationship> {
        update(relationship: Models.IDataListRelationship): Models.IDataListRelationship;
    }

    DataListRelationshipResource.$inject = ["$resource"];
    export function DataListRelationshipResource($resource: ng.resource.IResourceService): IDataListRelationshipResource {

        return <IDataListRelationshipResource>$resource('/api/datalists/:ownerId/relationships/:id',
            { ownerId: '@ownerId', id: '@id' },
            {
                'update': { method: 'PUT' }
            });

    }

    angular.module("app").factory("dataListRelationshipResource", DataListRelationshipResource);
}