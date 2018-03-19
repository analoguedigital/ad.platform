module App.Resources {
    "use strict";

    export interface IDataListItemResource extends ng.resource.IResourceClass<Models.IDataListItem> {
        update(project: Models.IDataListItem, success: Function, error?: Function): Models.IDataListItem;
    }

    DataListItemResource.$inject = ["$resource"];
    export function DataListItemResource($resource: ng.resource.IResourceService): IDataListItemResource {
        return <IDataListItemResource>$resource('/api/datalists/:dataListId/items/:id',
            { dataListId: '@dataListId', id: '@id' },
            {
                'update': { method: 'PUT' }
            });
    }

    angular.module("app").factory("dataListItemResource", DataListItemResource);
}