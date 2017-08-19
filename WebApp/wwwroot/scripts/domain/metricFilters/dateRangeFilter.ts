module App.Models {
    "use strict";

    export interface IDateRangeFilter extends IMetricFilter {
        startDate?: Date;
        endDate?: Date;
        canSelectTime: boolean;
    }

}