module App.Models {
    "use strict";

    export interface IFilterValue {
        id: string;
        type: string;
        shortTitle: string;
    }

    export const FilterValueTypes = {
        SingleFilterValue: 'single',
        RangeFilterValue: 'range',
        MultipleFilterValue: 'multiple'
    }

}