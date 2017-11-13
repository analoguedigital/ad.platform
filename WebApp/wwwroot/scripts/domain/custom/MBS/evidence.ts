
module App.Models.MBS {
    "use strict";

    export interface IEvidence extends ng.resource.IResource<IEvidence> {
        date: Date;
        comments: string;
        attachments: IAttachment[];
    }
}