module App.Resources {
    "use strict";

    export interface IPlatformUserResource extends ng.resource.IResourceClass<Models.IUser> {
        create(params: Object, success: Function, error?: Function): Models.IUser;
        update(params: Object, success: Function, error?: Function): Models.IUser;
    }

    PlatformUserResource.$inject = ["$resource"];
    export function PlatformUserResource($resource: ng.resource.IResourceService): IPlatformUserResource {
        var _resource = <IPlatformUserResource>$resource('/api/platform-users/:id', { id: '@id' }, {
            'create': { method: 'POST', url: '/api/platform-users/' },
            'update': { method: 'PUT' }
        });

        return _resource;
    }

    angular.module("app").factory("platformUserResource", PlatformUserResource);
}