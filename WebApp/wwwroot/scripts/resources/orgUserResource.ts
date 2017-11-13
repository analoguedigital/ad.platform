﻿
module App.Resources {
    "use strict";

    export interface IOrgUserResource extends ng.resource.IResourceClass<Models.IOrgUser> {
        update(user: Models.IOrgUser, success: Function, error?: Function): Models.IOrgUser;
    }

    export interface IUserResource extends ng.resource.IResourceClass<Models.IUser> { }

    OrgUserResource.$inject = ["$resource"];
    export function OrgUserResource($resource: ng.resource.IResourceService): IOrgUserResource {

        var OrgUser = <IOrgUserResource>$resource('/api/orgUsers/:id', { id: '@id' }, {
            'update': { method: 'PUT' }
        });

        OrgUser.prototype.toString = function OrgUser_toString() {
            if (this.firstName || this.surname) {
                var val = this.firstName;

                if (this.firstName && this.surname) {
                    val += ' ' + this.surname;
                }
                return val;
            } else {
                return this.email;
            }
        };

        return OrgUser;
        
    }

    UserResource.$inject = ["$resource"];
    export function UserResource($resource: ng.resource.IResourceService): IUserResource {
        return <IUserResource>$resource('/api/users/:id', { id: '@id' });
    }

    angular.module("app").factory("userResource", UserResource);
    angular.module("app").factory("orgUserResource", OrgUserResource);
}