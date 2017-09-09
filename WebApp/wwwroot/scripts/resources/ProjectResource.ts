
module App.Resources {
    "use strict";

    export interface IProjectResource extends ng.resource.IResourceClass<Models.IProject> {
        update(project: Models.IProject, success: Function, error?: Function): Models.IProject;
        assignments(params: Object, success: Function, error?: Function): Models.IProjectAssignment[];
        assign(params: Object, success: Function, error?: Function);
        unassign(params: Object, success: Function, error?: Function);
    }

    ProjectResource.$inject = ["$resource"];
    export function ProjectResource($resource: ng.resource.IResourceService): IProjectResource {

        return <IProjectResource>$resource('/api/projects/:id', { id: '@id' }, {
            'update': { method: 'PUT' },
            'assignments': { method: 'GET', isArray: true, url: '/api/projects/:id/assignments', params: { id: '@id' } },
            'assign': { method: 'POST', url: '/api/projects/:id/assign/:userId', params: { id: '@id', userId: '@userId', canAdd: '@canAdd', canEdit: '@canEdit', canView: '@canView', canDelete: '@canDelete' } },
            'unassign': { method: 'DELETE', url: '/api/projects/:id/assign/:userId', params: { id: '@id', userId: '@userId' } }
        });
    }


    angular.module("app").factory("projectResource", ProjectResource);
}