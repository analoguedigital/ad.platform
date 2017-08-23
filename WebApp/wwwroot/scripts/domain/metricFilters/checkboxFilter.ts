module App.Models {
    "use strict";

    export interface ICheckboxFilter extends IMetricFilter {
        dataList: IMetricFilterOption[];
    }

}