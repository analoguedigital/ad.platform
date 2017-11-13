
module App.Models {
    "use strict";

    export interface IMetricGroup extends ng.resource.IResource<IMetricGroup> {

        id: string;
        title: string;
        page: number;
        helpContext: string;
        isRepeater: boolean;
        isDataListRepeater: boolean;
        isAdHoc: boolean;
        adHocItems: IDataListItem[];
        type: string;
        dataListId: string;
        numberOfRows: number;
        canAddMoreRows: boolean;
        order: number;
        formTemplateId: string;
        metrics: IMetric[];
        isDeleted: boolean;
    }
}