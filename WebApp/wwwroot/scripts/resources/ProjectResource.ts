module App.Resources {
    "use strict";

    export interface IProjectResource extends ng.resource.IResourceClass<Models.IProject> {
        update(project: Models.IProject, success: Function, error?: Function): Models.IProject;
        assignments(params: Object, success: Function, error?: Function): Models.IProjectAssignment[];
        assign(params: Object, success: Function, error?: Function);
        unassign(params: Object, success: Function, error?: Function);
        teams(params: Object, success: Function, error?: Function);
        createAdviceThread(params: Object, success: Function, error?: Function);
        createRecordThread(params: Object, success: Function, error?: Function);
        getSharedProjects(): Models.IProject[];
        getDirect(params: Object): Models.IProject;
        getByUserId(params: Object): Models.IProject;
    }

    ProjectResource.$inject = ["$resource"];
    export function ProjectResource($resource: ng.resource.IResourceService): IProjectResource {

        return <IProjectResource>$resource('/api/projects/:id', { id: '@id' }, {
            'update': { method: 'PUT' },
            'assignments': { method: 'GET', isArray: true, url: '/api/projects/:id/assignments', params: { id: '@id' } },
            'assign': { method: 'POST', url: '/api/projects/:id/assign/:userId/:accessLevel', params: { id: '@id', userId: '@userId', accessLevel: '@accessLevel' } },
            'unassign': { method: 'DELETE', url: '/api/projects/:id/assign/:userId/:accessLevel', params: { id: '@id', userId: '@userId', accessLevel: '@accessLevel' } },
            'teams': { method: 'GET', isArray: true, url: '/api/projects/:id/teams', params: { id: '@id' } },
            'createAdviceThread': { method: 'POST', url: '/api/projects/:id/create-advice-thread', params: { id: '@id' } },
            'createRecordThread': { method: 'POST', url: '/api/projects/:id/create-record-thread', params: { id: '@id' } },
            'getSharedProjects': { method: 'GET', url: '/api/projects/shared', isArray: true },
            'getDirect': { method: 'GET', url: '/api/projects/direct/:id', params: { id: '@id' } },
            'getByUserId': { method: 'GET', url: '/api/projects/user/:orgUserId', params: { orgUserId: '@orgUserId' } }
        });
    }

    angular.module("app").factory("projectResource", ProjectResource);
}