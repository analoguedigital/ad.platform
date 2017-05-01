module App.Models {
    "use strict";

    export interface IRateMetric extends IMetric {
        minValue: number;
        maxValue: number;
    }
}