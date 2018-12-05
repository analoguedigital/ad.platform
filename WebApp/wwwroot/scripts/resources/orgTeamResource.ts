﻿module App.Resources {
    "use strict";

    export interface IOrgTeamResource extends ng.resource.IResourceClass<Models.IOrgTeam> {
        update(project: Models.IOrgTeam, success: Function, error?: Function): Models.IOrgTeam;
        assignments(params: Object, success: Function, error?: Function): Models.IProjectAssignment[];
        assign(params: Object, success: Function, error?: Function);
        getAssignableUsers(params: Object, success: Function, error?: Function);
        removeUser(params: Object, success: Function, error?: Function);
        updateStatus(params: Object, success: Function, error?: Function);
        getProjects(params: Object, success: Function, error?: Function): Models.IProject[];
        getUserTeams(params: Object, success: Function, error?: Function): Models.IProject[];
        updatePermissions(params: Object, success: Function, error?: Function);
    }

    OrgTeamResource.$inject = ["$resource"];
    export function OrgTeamResource($resource: ng.resource.IResourceService): IOrgTeamResource {

        return <IOrgTeamResource>$resource('/api/orgteams/:id', { id: '@id' }, {
            'update': { method: 'PUT' },
            'assignments': { method: 'POST', url: '/api/orgteams/:id/assignments', params: { id: '@id' }, isArray: true },
            'assign': { method: 'POST', url: '/api/orgteams/:id/assign/', params: { id: '@id' } },
            'getAssignableUsers': { method: 'GET', url: '/api/orgteams/:id/assignableusers', params: { id: '@id' }, isArray: true },
            'removeUser': { method: 'DELETE', url: '/api/orgteams/:id/removeuser/:userId', params: { id: '@id', userId: '@userId' } },
            'updateStatus': { method: 'POST', url: '/api/orgteams/:id/updatestatus/:userId/:flag', params: { id: '@id', userId: '@userId', flag: '@flag' } },
            'getProjects': { method: 'GET', url: '/api/orgteams/:id/projects', params: { id: '@id' }, isArray: true },
            'getUserTeams': { method: 'GET', url: '/api/orgteams/getuserteams/:userId', params: { userId: '@userId' }, isArray: true },
            'updatePermissions': { method: 'POST', url: '/api/orgteams/:id/updatePermissions/', params: { id: '@id' } }
        });
    }

    angular.module("app").factory("orgTeamResource", OrgTeamResource);
}