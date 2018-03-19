module App.Resources {
    "use strict";

    export interface IAchievementSummaryResource extends ng.resource.IResourceClass<Models.MBS.IAchievementSummary> { }

    AchievementSummaryResource.$inject = ["$resource"];
    export function AchievementSummaryResource($resource: ng.resource.IResourceService): IAchievementSummaryResource {
        return <IAchievementSummaryResource>$resource('/api/mbs/students/:projectId/achievementSummary',
            { projectId: "@projectId" });
    }

    angular.module("app").factory("achievementSummaryResource", AchievementSummaryResource);
}