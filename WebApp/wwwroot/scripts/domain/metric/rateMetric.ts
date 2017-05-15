module App.Models {
    "use strict";

    export interface IRateMetric extends IMetric {
        minValue: number;
        maxValue: number;
        defaultValue?: number;
        dataListId: string;
        isAdHoc: boolean;
        adHocItems: IDataListItem[];
    }
}