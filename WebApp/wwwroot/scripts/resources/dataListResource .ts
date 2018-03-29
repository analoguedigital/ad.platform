module App.Resources {
    "use strict";

    export interface IDataListResource extends angular.resource.IResourceClass<Models.IDataList> {
        update(datalist: Models.IDataList, success: Function, error?: Function): Models.IDataList;
        getReferences(params:any): Models.IDataListBasic[];
        list(): Models.IDataListBasic[];
        addRelationship(datalist);
    }

    DataListResource.$inject = ["$resource"];
    export function DataListResource($resource: angular.resource.IResourceService): IDataListResource {
        return <IDataListResource>$resource('/api/datalists/:id', { id: '@id' }, {
            'update': { method: 'PUT' },
            'getReferences': {
                method: 'GET',
                isArray: true,
                url: '/api/datalists/:id/references',
                transformResponse: function (data) {
                    var wrappedResult = angular.fromJson(data);
                    wrappedResult.items.$metadata = wrappedResult.metadata;
                    return wrappedResult.items;
                },

                interceptor: {
                    response: function (response) {
                        response.resource.$metadata = response.data.$metadata;
                        return response.resource;
                    }
                }
            },
            'list': {
                method: 'GET',
                isArray: true,

                transformResponse: function (data) {
                    var wrappedResult = angular.fromJson(data);
                    wrappedResult.items.$metadata = wrappedResult.metadata;
                    return wrappedResult.items;
                },

                interceptor: {
                    response: function (response) {
                        response.resource.$metadata = response.data.$metadata;
                        return response.resource;
                    }
                }
            }
        });
    }
    
    angular.module("app").factory("dataListResource", DataListResource);
}