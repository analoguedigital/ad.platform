module App.Models {
    "use strict";

    export interface IMultipleChoiceMetric extends IMetric {
        viewType: string;
        dataListId: string;
        isAdHoc: boolean;
        adHocItems: IDataListItem[];
    }
}