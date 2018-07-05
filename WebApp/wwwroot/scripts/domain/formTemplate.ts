module App.Models {
    "use strict";

    export enum FormTemplateDiscriminators {
        regularThread = 0,
        adviceThread = 1
    };

    export interface IFormTemplate extends angular.resource.IResource<IFormTemplate> {
        id: string;
        projectId: string;
        organisation: IOrganisation;
        code: string;
        title: string;
        project: IProject;
        projectName: string;
        description: string;
        colour: string;
        formTemplateCategory: IFormTemplateCategory;
        discriminator: FormTemplateDiscriminators;
        metricGroups: IMetricGroup[];
        isChecked: boolean;
        calendarDateMetricId: string;
        descriptionFormat: string;
        timelineBarMetricId: string;
        canView?: boolean;
        canAdd?: boolean;
        canEdit?: boolean;
        canDelete?: boolean;
    }

    export interface IThreadAssignment {
        formTemplateId: string;
        orgUserId: string;
        orgUserName: string;
        accountType: number;
        email: string;
        isRootUser: boolean;
        canAdd: boolean;
        canEdit: boolean;
        canView: boolean;
        canDelete: boolean;
    }
}