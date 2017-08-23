module App.Models {
    "use strict";

    export interface ITimeRangeFilter extends IMetricFilter {
        startTime?: Date;
        endTime?: Date;
    }

}