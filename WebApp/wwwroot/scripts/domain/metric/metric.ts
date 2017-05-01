module App.Models {
    "use strict";

    export interface IMetric extends ng.resource.IResource<IMetric> {
        id: string;
        type: string;
        shortTitle: string;
        description: string;
        metricGroupId: string;
        mandatory: boolean;
        sectionTitle: string;
        order: number;
        isDeleted: boolean;
    }
}