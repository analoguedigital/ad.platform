module App.Resources {
    "use strict";

    export interface IDownloadResource extends ng.resource.IResourceClass<any> {
        requestFile(params: Object, success: Function, error?: Function);
    }

    DownloadResource.$inject = ["$resource"];
    export function DownloadResource($resource: ng.resource.IResourceService): IDownloadResource {
        return <IDownloadResource>$resource('/api/downloads/:id', { id: '@id' }, {
            'requestFile': { method: 'GET', url: '/api/downloads/file/:id', params: { id: '@id' } }
        });
    }

    angular.module("app").factory("downloadResource", DownloadResource);
}