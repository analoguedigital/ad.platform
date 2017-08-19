module App.Models {
    "use strict";

    export interface IMetricFilter {
        metricId: string;
        shortTitle: string;
        type: string;
    }

    export interface IMetricFilterOption {
        text: string;
        value: number;
        selected: boolean;
    }

    export const MetricFilterTypes = {
        Text: 'Text',
        Checkbox: 'Checkbox',
        DateRange: 'DateRange',
        NumericRange: 'NumericRange',
        TimeRange: 'TimeRange'
    }

}