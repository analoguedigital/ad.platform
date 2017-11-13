module App.Models {
    "use strict";

    export interface ITextFilter extends IMetricFilter {
        maxLength: number;
        numberOfLines: number;
    }

}