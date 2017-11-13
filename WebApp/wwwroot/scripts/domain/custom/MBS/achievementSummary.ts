
module App.Models.MBS {
    "use strict";

    export interface IAchievementSummary extends ng.resource.IResource<IAchievementSummary> {
        projectId: string;
        achievementId: string;
        numberOfTargets: number;
        numberOfEvidenceItems: number;
    }
}