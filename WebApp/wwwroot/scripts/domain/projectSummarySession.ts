
module App.Models {
    "use strict";

    export interface IProjectSummaryPrintSession extends ng.resource.IResource<IProjectSummaryPrintSession> {

        id: string;
        projectId: string;
        surveyIds: string[];
        removedItemIds: string[];

    }

}