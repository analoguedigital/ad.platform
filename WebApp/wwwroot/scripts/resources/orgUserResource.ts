module App.Resources {
    "use strict";

    export interface IOrgUserResource extends ng.resource.IResourceClass<Models.IOrgUser> {
        getOnRecordStaff(success: Function, error?: Function): Models.IOrgUser[];
        getStaffMembers(params: Object, success: Function, error?: Function): Models.IOrgUser[];
        update(user: Models.IOrgUser, success: Function, error?: Function): Models.IOrgUser;
        deleteAccount(params: Object, success: Function, error?: Function);
        updateStatus(params: Object, success: Function, error?: Function);
    }

    OrgUserResource.$inject = ["$resource"];
    export function OrgUserResource($resource: ng.resource.IResourceService): IOrgUserResource {
        var OrgUser = <IOrgUserResource>$resource('/api/orgUsers/:listType/:id/', { listType: '@listType', id: '@id' }, {
            'getOnRecordStaff': { method: 'GET', url: '/api/orgusers/onrecord-staff', isArray: true },
            'getStaffMembers': { method: 'GET', url: '/api/orgusers/staff-members', isArray: true },
            'update': { method: 'PUT' },
            'deleteAccount': { method: 'POST', url: '/api/orgusers/deleteAccount/:id', params: { id: '@id' } },
            'updateStatus': { method: 'POST', url: '/api/orgusers/:userId/updateStatus/:isAuthorizedStaff', params: { userId: '@userId', isAuthorizedStaff: '@isAuthorizedStaff' } }
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

    angular.module("app").factory("orgUserResource", OrgUserResource);
}