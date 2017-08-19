module App.Models {
    "use strict";

    export interface INumericRangeFilter extends IMetricFilter {
        minValue?: number;
        maxValue?: number;
    }

}