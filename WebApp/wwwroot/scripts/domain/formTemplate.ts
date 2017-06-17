
module App.Models {
    "use strict";

    export interface IFormTemplate extends angular.resource.IResource<IFormTemplate> {
        id: string;
        projectId: string;
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
    }
}