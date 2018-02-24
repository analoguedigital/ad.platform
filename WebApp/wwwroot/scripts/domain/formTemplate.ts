
module App.Models {
    "use strict";

    export interface IFormTemplate extends angular.resource.IResource<IFormTemplate> {
        id: string;
        projectId: string;
        organisation: IOrganisation;
        code: string;
        title: string;
        projectName: string;
        description: string;
        colour: string;
        formTemplateCategory: IFormTemplateCategory;
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
        orgUserId: string;
        orgUserName: string;
        isRootUser: boolean;
        formTemplateId: string;
        canAdd: boolean;
        canEdit: boolean;
        canView: boolean;
        canDelete: boolean;
    }
}