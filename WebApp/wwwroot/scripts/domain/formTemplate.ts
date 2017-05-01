
module App.Models {
    "use strict";

    export interface IFormTemplate extends angular.resource.IResource<IFormTemplate> {
        id: string;
        projectId: string;
        code: string;
        title: string;
        projectName: string;
        description: string;
        formTemplateCategory: IFormTemplateCategory;
        metricGroups: IMetricGroup[];
    }
}