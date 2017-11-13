module App.Models {
    "use strict";

    export interface INumericMetric extends IMetric {
        minVal: number;
        maxVal: number;
    }
}