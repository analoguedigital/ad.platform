module App.Models {
    "use strict";

    export interface IAttachmentMetric extends IMetric {
        allowMultipleFiles: boolean;
        allowedAttachmentTypes: string[];
    }
}