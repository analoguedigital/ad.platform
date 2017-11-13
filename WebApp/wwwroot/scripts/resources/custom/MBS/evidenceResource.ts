
module App.Resources {
    "use strict";

    export interface IEvidenceResource extends ng.resource.IResourceClass<Models.MBS.IEvidence> {
    }

    EvidenceResource.$inject = ["$resource"];
    export function EvidenceResource($resource: ng.resource.IResourceService): IEvidenceResource {

        return <IEvidenceResource>$resource('/api/mbs/students/:projectId/evidences',
            { projectId: "@projectId" });
    }

    angular.module("app").factory("evidenceResource", EvidenceResource);
}