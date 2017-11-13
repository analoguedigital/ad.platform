
module App.Models {
    "use strict";

    export class FormValue {

        id: string;
        metricId: string;
        rowNumber: number;
        rowDataListItemId: string;
        textValue: string;
        boolValue: boolean;
        numericValue: number;
        dateValue: Date;
        guidValue: string;
        timeValue: Date;
        
        attachments: IAttachment[];
    }
}