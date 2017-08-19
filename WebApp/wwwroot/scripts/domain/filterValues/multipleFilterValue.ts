module App.Models {
    "use strict";

    export interface IMultipleFilterValue extends IFilterValue {
        options: IMetricFilterOption[];
        values: number[];
    }

}