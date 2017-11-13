module App.Models {
    "use strict";

    export interface IFreeTextMetric extends IMetric {
        numberOfLine: number;
        minLength: number;
        maxLength: number;
    }
}