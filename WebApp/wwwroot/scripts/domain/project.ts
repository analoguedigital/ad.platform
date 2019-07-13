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
        organisation: Models.IOrganisation;
        createdBy: Models.IOrgUser;
        allowView: boolean;
        allowAdd: boolean;
        allowEdit: boolean;
        allowDelete: boolean;
        allowExportPdf: boolean;
        allowExportZip: boolean;
        lastEntry?: Date;
        assignmentsCount: number;
        teamsCount: number;
        isAggregate: boolean;

        isSelected: boolean;
    }

    export interface IProjectAssignment {
        orgUserId: string;
        orgUserName: string;
        isRootUser: boolean;
        isOwner: boolean;
        projectId: string;
        canAdd: boolean;
        canEdit: boolean;
        canView: boolean;
        canDelete: boolean;
        canExportPdf: boolean;
        canExportZip: boolean;
    }
}