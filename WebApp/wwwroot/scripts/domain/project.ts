
module App.Models {
    "use strict";

    export interface IProject extends ng.resource.IResource<IProject> {

        id: string;
        number: string;
        name: string;
        startDate: Date;
        endDate: Date;
        notes: string;
        archived: boolean;

    }

    export interface IProjectAssignment {
        orgUserId: string;
        orgUserName: string;
        projectId: string;
        canAdd: boolean;
        canEdit: boolean;
        canView: boolean;
        canDelete: boolean;
    }
}