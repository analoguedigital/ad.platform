module App.Resources {
    "use strict";

    export interface IUserResource extends ng.resource.IResourceClass<Models.IUser> {
        createSuperUser(params: Object, success: Function, error?: Function): Models.IUser;
        update(params: Object, success: Function, error?: Function): Models.IUser;
    }

    UserResource.$inject = ["$resource"];
    export function UserResource($resource: ng.resource.IResourceService): IUserResource {
        var _resource = <IUserResource>$resource('/api/users/:id', { id: '@id' }, {
            'createSuperUser': { method: 'POST', url: '/api/users/createsuperuser' },
            'update': { method: 'PUT' }
        });

        return _resource;
    }

    angular.module("app").factory("userResource", UserResource);
}